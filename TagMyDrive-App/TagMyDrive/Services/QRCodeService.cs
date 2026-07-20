using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using MySqlConnector;
using QRCoder;

namespace TagMyDrive.Services
{
    public class QRCodeService
    {
        private readonly DatabaseService _dbService;

        public class QRCodeInfo
        {
            public int Id { get; set; }
            public int DiskId { get; set; }
            public int UserId { get; set; }
            public string QrCodeUrl { get; set; }
            public string HtmlExportFilename { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsActive { get; set; }
            public int DownloadCount { get; set; }

            public override string ToString()
            {
                return $"{HtmlExportFilename} - {CreatedAt:yyyy-MM-dd HH:mm} (Downloads: {DownloadCount})";
            }
        }

        public QRCodeService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public static Bitmap GenerateQRCodeImage(string content, int pixelSize = 10)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    return qrCode.GetGraphic(pixelSize, Color.Black, Color.White, true);
                }
            }
        }

        public async Task<(bool success, string message, int? qrCodeId)> GenerateQRCodeAsync(
            int diskId, int userId, string htmlExportFilename, string targetUrl = "")
        {
            try
            {
                if (string.IsNullOrEmpty(targetUrl))
                {
                    targetUrl = $"https://tagmydrive.com/view/{diskId}";
                }

                var insertQuery = "INSERT INTO qr_codes (disk_id, user_id, qr_code_url, html_export_filename, expires_at) " +
                                 "VALUES (@diskId, @userId, @qrCodeUrl, @htmlFilename, DATE_ADD(NOW(), INTERVAL 1 YEAR))";

                await _dbService.ExecuteNonQueryAsync(insertQuery,
                    new MySqlParameter("@diskId", diskId),
                    new MySqlParameter("@userId", userId),
                    new MySqlParameter("@qrCodeUrl", targetUrl),
                    new MySqlParameter("@htmlFilename", htmlExportFilename ?? ""));

                var getIdQuery = "SELECT id FROM qr_codes WHERE disk_id = @diskId AND user_id = @userId ORDER BY created_at DESC LIMIT 1";
                var qrCodeId = Convert.ToInt32(await _dbService.ExecuteScalarAsync(getIdQuery,
                    new MySqlParameter("@diskId", diskId),
                    new MySqlParameter("@userId", userId)));

                return (true, "QR code generated successfully.", qrCodeId);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to generate QR code: {ex.Message}", null);
            }
        }

        public async Task<Bitmap> GetQRCodeImageAsync(int qrCodeId)
        {
            try
            {
                var query = "SELECT qr_code_url FROM qr_codes WHERE id = @qrCodeId";
                var result = await _dbService.ExecuteScalarAsync(query,
                    new MySqlParameter("@qrCodeId", qrCodeId));

                if (result != null && result != DBNull.Value && !string.IsNullOrEmpty(result.ToString()))
                {
                    return GenerateQRCodeImage(result.ToString());
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get QR code image: {ex.Message}", ex);
            }
        }

        public async Task<List<QRCodeInfo>> GetDiskQRCodesAsync(int diskId)
        {
            var qrCodes = new List<QRCodeInfo>();
            try
            {
                var query = "SELECT id, disk_id, user_id, qr_code_url, html_export_filename, created_at, is_active, download_count " +
                           "FROM qr_codes WHERE disk_id = @diskId ORDER BY created_at DESC";

                using (var reader = await _dbService.ExecuteReaderAsync(query,
                    new MySqlParameter("@diskId", diskId)))
                {
                    while (await reader.ReadAsync())
                    {
                        qrCodes.Add(new QRCodeInfo
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            DiskId = Convert.ToInt32(reader["disk_id"]),
                            UserId = Convert.ToInt32(reader["user_id"]),
                            QrCodeUrl = reader["qr_code_url"]?.ToString(),
                            HtmlExportFilename = reader["html_export_filename"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            IsActive = Convert.ToBoolean(reader["is_active"]),
                            DownloadCount = Convert.ToInt32(reader["download_count"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get QR codes: {ex.Message}", ex);
            }

            return qrCodes;
        }

        public async Task<bool> IncrementDownloadCountAsync(int qrCodeId)
        {
            try
            {
                var query = "UPDATE qr_codes SET download_count = download_count + 1, last_accessed = NOW() WHERE id = @qrCodeId";
                var result = await _dbService.ExecuteNonQueryAsync(query,
                    new MySqlParameter("@qrCodeId", qrCodeId));
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(bool success, string message)> DeactivateQRCodeAsync(int qrCodeId)
        {
            try
            {
                var query = "UPDATE qr_codes SET is_active = FALSE WHERE id = @qrCodeId";
                var result = await _dbService.ExecuteNonQueryAsync(query,
                    new MySqlParameter("@qrCodeId", qrCodeId));
                return (result > 0, result > 0 ? "QR code deactivated." : "QR code not found.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed: {ex.Message}");
            }
        }

        public async Task DeactivateAllForDiskAsync(int diskId)
        {
            try
            {
                var query = "UPDATE qr_codes SET is_active = FALSE WHERE disk_id = @diskId AND is_active = TRUE";
                await _dbService.ExecuteNonQueryAsync(query,
                    new MySqlParameter("@diskId", diskId));
            }
            catch { }
        }
    }
}
