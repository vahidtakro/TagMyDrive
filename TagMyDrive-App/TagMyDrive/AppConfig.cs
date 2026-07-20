using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace TagMyDrive
{
    public static class AppConfig
    {
        private static readonly string SessionFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TagMyDrive", "session.dat");

        private static readonly string EnvFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, ".env.local");

        private static readonly string EnvFilePathAlt = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, ".env");

        private static Dictionary<string, string> _envCache;

        private static Dictionary<string, string> LoadEnvFile()
        {
            if (_envCache != null) return _envCache;
            _envCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var path in new[] { EnvFilePath, EnvFilePathAlt })
            {
                if (!File.Exists(path)) continue;
                foreach (var rawLine in File.ReadAllLines(path))
                {
                    var line = rawLine.Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;
                    var eqIndex = line.IndexOf('=');
                    if (eqIndex <= 0) continue;
                    var key = line.Substring(0, eqIndex).Trim();
                    var value = line.Substring(eqIndex + 1).Trim();
                    if (value.Length >= 2 && ((value[0] == '"' && value[value.Length - 1] == '"') || (value[0] == '\'' && value[value.Length - 1] == '\'')))
                        value = value.Substring(1, value.Length - 2);
                    if (!_envCache.ContainsKey(key))
                        _envCache[key] = value;
                }
            }
            return _envCache;
        }

        private static string GetEnv(string key)
        {
            var sysVal = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrEmpty(sysVal))
                return sysVal;

            var envFile = LoadEnvFile();
            if (envFile.TryGetValue(key, out var fileVal) && !string.IsNullOrEmpty(fileVal))
                return fileVal;

            return null;
        }

        public static string GetDatabaseConnectionString()
        {
            return GetEnv("TAGMYDRIVE_DB_CONNECTION")
                ?? ConfigurationManager.ConnectionStrings["TagMyDriveDb"]?.ConnectionString
                ?? "";
        }

        public static string GetGoogleDriveClientId()
        {
            return GetEnv("TAGMYDRIVE_GOOGLE_CLIENT_ID")
                ?? ConfigurationManager.AppSettings["GoogleDriveClientId"]
                ?? "";
        }

        public static string GetGoogleDriveClientSecret()
        {
            return GetEnv("TAGMYDRIVE_GOOGLE_CLIENT_SECRET")
                ?? ConfigurationManager.AppSettings["GoogleDriveClientSecret"]
                ?? "";
        }

        public static string GetGoogleDriveRedirectUri()
        {
            return ConfigurationManager.AppSettings["GoogleDriveRedirectUri"] ?? "http://localhost:8080/authorize";
        }

        public static bool IsGoogleDriveConfigured()
        {
            return !string.IsNullOrWhiteSpace(GetGoogleDriveClientId()) &&
                   !string.IsNullOrWhiteSpace(GetGoogleDriveClientSecret());
        }

        public static string GetCryptoWalletUSDT()
        {
            return GetEnv("TAGMYDRIVE_CRYPTO_WALLET_USDT")
                ?? ConfigurationManager.AppSettings["CryptoWalletUSDT"]
                ?? "";
        }

        public static string GetBscScanApiKey()
        {
            return GetEnv("TAGMYDRIVE_BSCSCAN_API_KEY")
                ?? ConfigurationManager.AppSettings["BscScanApiKey"]
                ?? "";
        }

        public static bool IsCryptoPaymentEnabled()
        {
            var val = ConfigurationManager.AppSettings["CryptoPaymentEnabled"] ?? "true";
            return val.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        public static void SaveSession(int userId)
        {
            try
            {
                var dir = Path.GetDirectoryName(SessionFilePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(SessionFilePath, userId.ToString());
            }
            catch { }
        }

        public static int LoadSession()
        {
            try
            {
                if (File.Exists(SessionFilePath))
                {
                    var text = File.ReadAllText(SessionFilePath).Trim();
                    if (int.TryParse(text, out int userId) && userId > 0)
                        return userId;
                }
            }
            catch { }
            return 0;
        }

        public static void ClearSession()
        {
            try
            {
                if (File.Exists(SessionFilePath))
                    File.Delete(SessionFilePath);
            }
            catch { }
        }
    }
}
