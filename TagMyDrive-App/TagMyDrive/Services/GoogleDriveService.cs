using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using MySqlConnector;

namespace TagMyDrive.Services
{
    public class GoogleDriveService
    {
        private readonly DatabaseService _dbService;
        private DriveService _driveService;
        private UserCredential _credential;
        private string _currentUserEmail;

        public GoogleDriveService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public bool IsAuthenticated => _driveService != null;

        public async Task<(bool success, string message)> AuthenticateAsync(string clientId, string clientSecret, string redirectUri)
        {
            try
            {
                var clientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                };

                _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    new[] { DriveService.ScopeConstants.DriveFile },
                    "user",
                    System.Threading.CancellationToken.None,
                    null);

                _driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _credential,
                    ApplicationName = "TagMyDrive"
                });

                var about = await _driveService.About.Get().ExecuteAsync();
                _currentUserEmail = about.User.EmailAddress;

                return (true, $"Authenticated as {_currentUserEmail}");
            }
            catch (Exception ex)
            {
                return (false, $"Google Drive authentication failed: {ex.Message}");
            }
        }

        public async Task<(bool success, string folderId)> GetOrCreateSnapshotsFolderAsync()
        {
            try
            {
                if (_driveService == null)
                    return (false, null);

                var listRequest = _driveService.Files.List();
                listRequest.Q = "mimeType='application/vnd.google-apps.folder' and name='TagMyDrive Snapshots' and trashed=false";
                listRequest.Fields = "files(id, name)";
                var result = await listRequest.ExecuteAsync();

                if (result.Files != null && result.Files.Count > 0)
                    return (true, result.Files[0].Id);

                var folderMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = "TagMyDrive Snapshots",
                    MimeType = "application/vnd.google-apps.folder"
                };

                var createRequest = _driveService.Files.Create(folderMetadata);
                createRequest.Fields = "id";
                var folder = await createRequest.ExecuteAsync();
                return (true, folder.Id);
            }
            catch (Exception)
            {
                return (false, null);
            }
        }

        public async Task<(bool success, string message, string fileId, string fileUrl)> UploadFileAsync(
            string filePath, string fileName, string folderId = null)
        {
            try
            {
                if (_driveService == null)
                    return (false, "Not authenticated with Google Drive.", null, null);

                if (!File.Exists(filePath))
                    return (false, "File not found.", null, null);

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = fileName,
                    MimeType = "text/html"
                };

                if (!string.IsNullOrEmpty(folderId))
                    fileMetadata.Parents = new List<string> { folderId };

                FilesResource.CreateMediaUpload request;
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    request = _driveService.Files.Create(fileMetadata, stream, "text/html");
                    request.Fields = "id, webViewLink";
                    var uploadResult = await request.UploadAsync();

                    if (uploadResult.Status == Google.Apis.Upload.UploadStatus.Failed)
                        return (false, $"Upload failed: {uploadResult.Exception?.Message}", null, null);

                    var uploadedFile = request.ResponseBody;
                    string fileUrl = uploadedFile.WebViewLink;

                    return (true, "File uploaded successfully.", uploadedFile.Id, fileUrl);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Upload failed: {ex.Message}", null, null);
            }
        }

        public async Task<(bool success, string message)> SaveGoogleDriveLinkAsync(
            int diskId, int userId, int? qrCodeId, string googleFileId, string googleFileName,
            string googleDriveUrl, string htmlFileName)
        {
            try
            {
                var query = "INSERT INTO google_drive_links " +
                           "(disk_id, user_id, qr_code_id, google_file_id, google_file_name, google_drive_url, html_file_name, access_token, refresh_token, token_expires_at) " +
                           "VALUES (@diskId, @userId, @qrCodeId, @googleFileId, @googleFileName, @googleUrl, @htmlFileName, @accessToken, @refreshToken, DATE_ADD(NOW(), INTERVAL 1 HOUR))";

                var result = await _dbService.ExecuteNonQueryAsync(query,
                    new MySqlParameter("@diskId", diskId),
                    new MySqlParameter("@userId", userId),
                    new MySqlParameter("@qrCodeId", qrCodeId ?? (object)DBNull.Value),
                    new MySqlParameter("@googleFileId", googleFileId),
                    new MySqlParameter("@googleFileName", googleFileName),
                    new MySqlParameter("@googleUrl", googleDriveUrl),
                    new MySqlParameter("@htmlFileName", htmlFileName),
                    new MySqlParameter("@accessToken", _credential?.Token?.AccessToken ?? ""),
                    new MySqlParameter("@refreshToken", _credential?.Token?.RefreshToken ?? ""));

                return (result > 0, result > 0 ? "Saved." : "Failed to save.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to save link: {ex.Message}");
            }
        }

        public async Task<List<GoogleDriveLinkInfo>> GetDiskGoogleDriveLinksAsync(int diskId)
        {
            var links = new List<GoogleDriveLinkInfo>();
            try
            {
                var query = "SELECT id, disk_id, google_file_id, google_file_name, google_drive_url, uploaded_at " +
                           "FROM google_drive_links WHERE disk_id = @diskId AND is_active = TRUE ORDER BY uploaded_at DESC";

                using (var reader = await _dbService.ExecuteReaderAsync(query,
                    new MySqlParameter("@diskId", diskId)))
                {
                    while (await reader.ReadAsync())
                    {
                        links.Add(new GoogleDriveLinkInfo
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            DiskId = Convert.ToInt32(reader["disk_id"]),
                            GoogleFileId = reader["google_file_id"].ToString(),
                            GoogleFileName = reader["google_file_name"].ToString(),
                            GoogleDriveUrl = reader["google_drive_url"].ToString(),
                            UploadedAt = Convert.ToDateTime(reader["uploaded_at"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get Google Drive links: {ex.Message}", ex);
            }

            return links;
        }

        public class GoogleDriveLinkInfo
        {
            public int Id { get; set; }
            public int DiskId { get; set; }
            public string GoogleFileId { get; set; }
            public string GoogleFileName { get; set; }
            public string GoogleDriveUrl { get; set; }
            public DateTime UploadedAt { get; set; }

            public override string ToString()
            {
                return $"{GoogleFileName} - {UploadedAt:yyyy-MM-dd HH:mm}";
            }
        }

        public async Task DeactivateAllForDiskAsync(int diskId)
        {
            try
            {
                var query = "UPDATE google_drive_links SET is_active = FALSE WHERE disk_id = @diskId AND is_active = TRUE";
                await _dbService.ExecuteNonQueryAsync(query,
                    new MySqlParameter("@diskId", diskId));
            }
            catch { }
        }
    }
}
