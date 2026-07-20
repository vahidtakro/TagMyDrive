using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using MySqlConnector;
using QRCoder;

namespace TagMyDrive.Services
{
    public class CryptoPaymentService
    {
        private readonly DatabaseService _dbService;
        private static readonly HttpClient _httpClient = new HttpClient();

        private const string USDT_BEP20_CONTRACT = "0x55d398326f99059fF775485246999027B3197955";
        private const string TRANSFER_TOPIC = "0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef";
        private const int MIN_CONFIRMATIONS = 12;
        private const int USDT_DECIMALS = 6;

        public class PaymentInfo
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int MembershipId { get; set; }
            public string Coin { get; set; }
            public string WalletAddress { get; set; }
            public decimal Amount { get; set; }
            public string Status { get; set; }
            public string TransactionHash { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? PaidAt { get; set; }

            public override string ToString()
            {
                return $"{Amount} USDT - {Status} - {CreatedAt:yyyy-MM-dd HH:mm}";
            }
        }

        public CryptoPaymentService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public string GetWalletAddress()
        {
            return AppConfig.GetCryptoWalletUSDT();
        }

        public bool IsConfigured()
        {
            return !string.IsNullOrWhiteSpace(GetWalletAddress()) &&
                   !string.IsNullOrWhiteSpace(AppConfig.GetBscScanApiKey());
        }

        public decimal GetPriceForMembership(int membershipId)
        {
            switch (membershipId)
            {
                case 2: return 4.99m;
                case 3: return 9.99m;
                default: return 0;
            }
        }

        public async Task<int> CreatePaymentAsync(int userId, int membershipId)
        {
            try
            {
                var walletAddress = GetWalletAddress();
                var amount = GetPriceForMembership(membershipId);

                if (string.IsNullOrEmpty(walletAddress) || amount <= 0)
                    return 0;

                var query = "INSERT INTO crypto_payments (user_id, membership_id, coin, wallet_address, amount, status) " +
                           "VALUES (@userId, @membershipId, 'USDT', @walletAddress, @amount, 'pending')";

                await _dbService.ExecuteNonQueryAsync(query,
                    new MySqlParameter("@userId", userId),
                    new MySqlParameter("@membershipId", membershipId),
                    new MySqlParameter("@walletAddress", walletAddress),
                    new MySqlParameter("@amount", amount));

                var getIdQuery = "SELECT LAST_INSERT_ID()";
                var id = await _dbService.ExecuteScalarAsync(getIdQuery);
                return id != null ? Convert.ToInt32(id) : 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<(bool success, string message)> VerifyAndUpgradeAsync(int paymentId, string txHash)
        {
            try
            {
                var payment = await GetPaymentAsync(paymentId);
                if (payment == null)
                    return (false, "Payment not found.");

                if (payment.Status == "confirmed")
                    return (false, "Payment already confirmed.");

                if (payment.Status == "awaiting_verification" || payment.Status == "pending")
                {
                    var bscApiKey = AppConfig.GetBscScanApiKey();
                    if (string.IsNullOrWhiteSpace(bscApiKey))
                        return (false, "BSCScan API key not configured. Contact administrator.");

                    var walletAddress = payment.WalletAddress.ToLower();
                    var expectedAmount = payment.Amount;

                    string apiUrl = $"https://api.bscscan.com/api?module=proxy&action=eth_getTransactionReceipt&txhash={txHash}&apikey={bscApiKey}";
                    var response = await _httpClient.GetStringAsync(apiUrl);

                    if (string.IsNullOrEmpty(response) || response.Contains("\"result\":null") || response.Contains("\"result\":null"))
                        return (false, "Transaction not found. Please check the hash and try again.");

                    var receipt = ParseJson(response);

                    if (!receipt.ContainsKey("result") || receipt["result"] == null)
                        return (false, "Transaction not found on BSC network.");

                    var result = receipt["result"] as Dictionary<string, object>;
                    if (result == null)
                        return (false, "Invalid transaction data.");

                    string txTo = GetJsonString(result, "to")?.ToLower();
                    if (txTo != USDT_BEP20_CONTRACT.ToLower())
                        return (false, "Transaction is not a USDT BEP20 transfer.");

                    string blockNumberHex = GetJsonString(result, "blockNumber");
                    if (string.IsNullOrEmpty(blockNumberHex))
                        return (false, "Transaction is pending (not yet mined).");

                    long txBlock = Convert.ToInt64(blockNumberHex, 16);

                    string latestBlockUrl = $"https://api.bscscan.com/api?module=proxy&action=eth_blockNumber&apikey={bscApiKey}";
                    var latestResponse = await _httpClient.GetStringAsync(latestBlockUrl);
                    var latestParsed = ParseJson(latestResponse);
                    long latestBlock = Convert.ToInt64(GetJsonString(latestParsed, "result"), 16);
                    long confirmations = latestBlock - txBlock;

                    if (confirmations < MIN_CONFIRMATIONS)
                        return (false, $"Not enough confirmations yet. {confirmations}/{MIN_CONFIRMATIONS} confirmations. Please wait a moment and try again.");

                    var logs = GetJsonArray(result, "logs");
                    if (logs == null || logs.Count == 0)
                        return (false, "No logs found in transaction.");

                    bool foundTransfer = false;
                    foreach (var log in logs)
                    {
                        var logDict = log as Dictionary<string, object>;
                        if (logDict == null) continue;

                        string logAddress = GetJsonString(logDict, "address")?.ToLower();
                        if (logAddress != USDT_BEP20_CONTRACT.ToLower()) continue;

                        var topics = GetJsonArray(logDict, "topics");
                        if (topics == null || topics.Count < 3) continue;

                        string topic0 = topics[0]?.ToString().ToLower();
                        if (topic0 != TRANSFER_TOPIC) continue;

                        string toAddress = "0x" + topics[2].ToString().Substring(26);
                        if (toAddress.ToLower() != walletAddress) continue;

                        string dataHex = GetJsonString(logDict, "data");
                        if (string.IsNullOrEmpty(dataHex)) continue;

                        long amountRaw = Convert.ToInt64(dataHex, 16);
                        decimal amountUsdt = (decimal)amountRaw / (decimal)Math.Pow(10, USDT_DECIMALS);

                        if (Math.Abs(amountUsdt - expectedAmount) < 0.01m)
                        {
                            foundTransfer = true;
                            break;
                        }
                    }

                    if (!foundTransfer)
                        return (false, $"USDT transfer to your wallet not found for the expected amount ({expectedAmount} USDT). Please verify the amount sent.");

                    await UpdatePaymentStatusAsync(paymentId, "confirmed", txHash);

                    var membershipService = new MembershipService(_dbService);
                    await membershipService.UpgradeMembershipAsync(payment.UserId, payment.MembershipId);

                    return (true, $"Payment verified and membership upgraded to {(payment.MembershipId == 2 ? "Basic" : "Premium")}!");
                }

                return (false, $"Payment is in status '{payment.Status}'. Please contact administrator.");
            }
            catch (Exception ex)
            {
                return (false, $"Verification error: {ex.Message}");
            }
        }

        private async Task UpdatePaymentStatusAsync(int paymentId, string status, string txHash)
        {
            var query = "UPDATE crypto_payments SET status = @status, transaction_hash = @txHash, paid_at = NOW() WHERE id = @id";
            await _dbService.ExecuteNonQueryAsync(query,
                new MySqlParameter("@status", status),
                new MySqlParameter("@txHash", txHash),
                new MySqlParameter("@id", paymentId));
        }

        public async Task<PaymentInfo> GetPaymentAsync(int paymentId)
        {
            try
            {
                var query = "SELECT id, user_id, membership_id, coin, wallet_address, amount, status, transaction_hash, created_at, paid_at " +
                           "FROM crypto_payments WHERE id = @id";

                using (var reader = await _dbService.ExecuteReaderAsync(query,
                    new MySqlParameter("@id", paymentId)))
                {
                    if (await reader.ReadAsync())
                    {
                        return new PaymentInfo
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            UserId = Convert.ToInt32(reader["user_id"]),
                            MembershipId = Convert.ToInt32(reader["membership_id"]),
                            Coin = reader["coin"].ToString(),
                            WalletAddress = reader["wallet_address"].ToString(),
                            Amount = Convert.ToDecimal(reader["amount"]),
                            Status = reader["status"].ToString(),
                            TransactionHash = reader["transaction_hash"]?.ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            PaidAt = reader["paid_at"] as DateTime?
                        };
                    }
                }
            }
            catch { }

            return null;
        }

        public Bitmap GeneratePaymentQRCode(string address, decimal amount)
        {
            string qrContent = $"https://metamask.app.link/send/{address}?amount={amount}";
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    return qrCode.GetGraphic(10, Color.Black, Color.White, true);
                }
            }
        }

        private Dictionary<string, object> ParseJson(string json)
        {
            var dict = new Dictionary<string, object>();
            json = json.Trim();

            if (json.StartsWith("{"))
            {
                var content = json.Substring(1, json.Length - 2).Trim();
                var pairs = SplitJsonPairs(content);
                foreach (var pair in pairs)
                {
                    var colonIndex = pair.IndexOf(':');
                    if (colonIndex < 0) continue;
                    var key = pair.Substring(0, colonIndex).Trim().Trim('"');
                    var value = pair.Substring(colonIndex + 1).Trim();
                    dict[key] = ParseJsonValue(value);
                }
            }
            return dict;
        }

        private List<string> SplitJsonPairs(string content)
        {
            var pairs = new List<string>();
            int depth = 0;
            int start = 0;
            bool inString = false;
            bool escaped = false;

            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                if (escaped) { escaped = false; continue; }
                if (c == '\\') { escaped = true; continue; }
                if (c == '"') { inString = !inString; continue; }
                if (inString) continue;
                if (c == '{' || c == '[') depth++;
                if (c == '}' || c == ']') depth--;
                if (c == ',' && depth == 0)
                {
                    pairs.Add(content.Substring(start, i - start));
                    start = i + 1;
                }
            }
            if (start < content.Length)
                pairs.Add(content.Substring(start));

            return pairs;
        }

        private object ParseJsonValue(string value)
        {
            value = value.Trim();
            if (value == "null") return null;
            if (value == "true") return true;
            if (value == "false") return false;
            if (value.StartsWith("\""))
            {
                string s = value.Substring(1, value.Length - 2);
                s = s.Replace("\\\"", "\"").Replace("\\\\", "\\");
                return s;
            }
            if (value.StartsWith("{"))
            {
                return ParseJson(value);
            }
            if (value.StartsWith("["))
            {
                return ParseJsonArray(value);
            }
            try
            {
                if (value.Contains("."))
                    return double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                return value;
            }
            catch { return value; }
        }

        private List<object> ParseJsonArray(string json)
        {
            var list = new List<object>();
            json = json.Trim();
            if (!json.StartsWith("[")) return list;

            var content = json.Substring(1, json.Length - 2).Trim();
            if (string.IsNullOrEmpty(content)) return list;

            int depth = 0;
            int start = 0;
            bool inString = false;
            bool escaped = false;

            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                if (escaped) { escaped = false; continue; }
                if (c == '\\') { escaped = true; continue; }
                if (c == '"') { inString = !inString; continue; }
                if (inString) continue;
                if (c == '{' || c == '[') depth++;
                if (c == '}' || c == ']') depth--;
                if (c == ',' && depth == 0)
                {
                    list.Add(ParseJsonValue(content.Substring(start, i - start)));
                    start = i + 1;
                }
            }
            if (start < content.Length)
                list.Add(ParseJsonValue(content.Substring(start)));

            return list;
        }

        private string GetJsonString(Dictionary<string, object> dict, string key)
        {
            if (dict.ContainsKey(key) && dict[key] != null)
                return dict[key].ToString();
            return null;
        }

        private List<object> GetJsonArray(Dictionary<string, object> dict, string key)
        {
            if (dict.ContainsKey(key) && dict[key] is List<object>)
                return (List<object>)dict[key];
            return null;
        }
    }
}
