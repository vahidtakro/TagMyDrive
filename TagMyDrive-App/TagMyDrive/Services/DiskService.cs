using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

namespace TagMyDrive.Services
{
    public class DiskService
    {
        private readonly DatabaseService _dbService;

        public class Disk
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string DiskPath { get; set; }
            public string DiskType { get; set; }
            public string ImageUrl { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public bool IsActive { get; set; }

            public override string ToString()
            {
                return $"{Name} [{DiskType}] - {DiskPath}";
            }
        }

        public DiskService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        /// <summary>
        /// Creates a new disk entry for a user
        /// </summary>
        public async Task<(bool success, string message, int? diskId)> CreateDiskAsync(int userId, string name, string description = "", string diskPath = "", string diskType = "local", string imageUrl = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return (false, "Disk name is required.", null);
                }

                var limitQuery = "SELECT COUNT(d.id) as disk_count, m.max_disks " +
                               "FROM users u " +
                               "INNER JOIN memberships m ON u.membership_id = m.id " +
                               "LEFT JOIN disks d ON d.user_id = u.id AND d.is_active = TRUE " +
                               "WHERE u.id = @userId " +
                               "GROUP BY u.id";

                using (var reader = await _dbService.ExecuteReaderAsync(limitQuery,
                    new MySqlParameter("@userId", userId)))
                {
                    if (await reader.ReadAsync())
                    {
                        var currentCount = Convert.ToInt32(reader["disk_count"]);
                        var maxAllowed = Convert.ToInt32(reader["max_disks"]);
                        if (currentCount >= maxAllowed)
                        {
                            return (false, $"Disk limit reached ({currentCount}/{maxAllowed}). Upgrade your membership to add more disks.", null);
                        }
                    }
                }

                var checkQuery = "SELECT COUNT(*) FROM disks WHERE user_id = @userId AND name = @name AND is_active = TRUE";
                var exists = Convert.ToInt32(await _dbService.ExecuteScalarAsync(checkQuery,
                    new MySqlParameter("@userId", userId),
                    new MySqlParameter("@name", name)));

                if (exists > 0)
                {
                    return (false, "A disk with this name already exists.", null);
                }

                var insertQuery = !string.IsNullOrEmpty(imageUrl)
                    ? "INSERT INTO disks (user_id, name, description, disk_path, disk_type, image_url) VALUES (@userId, @name, @description, @diskPath, @diskType, @imageUrl)"
                    : "INSERT INTO disks (user_id, name, description, disk_path, disk_type) VALUES (@userId, @name, @description, @diskPath, @diskType)";

                var insertParams = new List<MySqlParameter>
                {
                    new MySqlParameter("@userId", userId),
                    new MySqlParameter("@name", name),
                    new MySqlParameter("@description", description ?? ""),
                    new MySqlParameter("@diskPath", diskPath ?? ""),
                    new MySqlParameter("@diskType", diskType ?? "local")
                };
                if (!string.IsNullOrEmpty(imageUrl))
                    insertParams.Add(new MySqlParameter("@imageUrl", imageUrl));

                await _dbService.ExecuteNonQueryAsync(insertQuery, insertParams.ToArray());

                var getIdQuery = "SELECT id FROM disks WHERE user_id = @userId AND name = @name ORDER BY created_at DESC LIMIT 1";
                var diskId = Convert.ToInt32(await _dbService.ExecuteScalarAsync(getIdQuery,
                    new MySqlParameter("@userId", userId),
                    new MySqlParameter("@name", name)));

                return (true, "Disk created successfully.", diskId);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to create disk: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Gets all disks for a user
        /// </summary>
        public async Task<List<Disk>> GetUserDisksAsync(int userId)
        {
            var disks = new List<Disk>();
            try
            {
                var query = "SELECT id, user_id, name, description, disk_path, disk_type, image_url, created_at, updated_at, is_active " +
                           "FROM disks WHERE user_id = @userId AND is_active = TRUE ORDER BY created_at DESC";

                using (var reader = await _dbService.ExecuteReaderAsync(query,
                    new MySqlParameter("@userId", userId)))
                {
                    while (await reader.ReadAsync())
                    {
                        disks.Add(new Disk
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            UserId = Convert.ToInt32(reader["user_id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString(),
                            DiskPath = reader["disk_path"].ToString(),
                            DiskType = reader["disk_type"].ToString(),
                            ImageUrl = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : null,
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            UpdatedAt = Convert.ToDateTime(reader["updated_at"]),
                            IsActive = Convert.ToBoolean(reader["is_active"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get disks: {ex.Message}", ex);
            }

            return disks;
        }

        /// <summary>
        /// Gets a specific disk by ID
        /// </summary>
        public async Task<Disk> GetDiskByIdAsync(int diskId)
        {
            try
            {
                var query = "SELECT id, user_id, name, description, disk_path, disk_type, image_url, created_at, updated_at, is_active " +
                           "FROM disks WHERE id = @diskId";

                using (var reader = await _dbService.ExecuteReaderAsync(query,
                    new MySqlParameter("@diskId", diskId)))
                {
                    if (await reader.ReadAsync())
                    {
                        return new Disk
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            UserId = Convert.ToInt32(reader["user_id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString(),
                            DiskPath = reader["disk_path"].ToString(),
                            DiskType = reader["disk_type"].ToString(),
                            ImageUrl = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : null,
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            UpdatedAt = Convert.ToDateTime(reader["updated_at"]),
                            IsActive = Convert.ToBoolean(reader["is_active"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get disk: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Updates disk information
        /// </summary>
        public async Task<(bool success, string message)> UpdateDiskAsync(int diskId, string name, string description = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return (false, "Disk name is required.");
                }

                var updateQuery = "UPDATE disks SET name = @name, description = @description, updated_at = NOW() WHERE id = @diskId";

                var result = await _dbService.ExecuteNonQueryAsync(updateQuery,
                    new MySqlParameter("@name", name),
                    new MySqlParameter("@description", description ?? ""),
                    new MySqlParameter("@diskId", diskId));

                if (result > 0)
                {
                    return (true, "Disk updated successfully.");
                }

                return (false, "Disk not found.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update disk: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a disk (soft delete)
        /// </summary>
        public async Task<(bool success, string message)> DeleteDiskAsync(int diskId)
        {
            try
            {
                var deleteQuery = "UPDATE disks SET is_active = FALSE, updated_at = NOW() WHERE id = @diskId";

                var result = await _dbService.ExecuteNonQueryAsync(deleteQuery,
                    new MySqlParameter("@diskId", diskId));

                if (result > 0)
                {
                    return (true, "Disk deleted successfully.");
                }

                return (false, "Disk not found.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete disk: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets count of active disks for a user
        /// </summary>
        public async Task<int> GetUserActiveDiskCountAsync(int userId)
        {
            try
            {
                var query = "SELECT COUNT(*) FROM disks WHERE user_id = @userId AND is_active = TRUE";
                var result = await _dbService.ExecuteScalarAsync(query,
                    new MySqlParameter("@userId", userId));

                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets an active disk by user and path
        /// </summary>
        public async Task<Disk> GetDiskByPathAsync(int userId, string diskPath)
        {
            try
            {
                var query = "SELECT id, user_id, name, description, disk_path, disk_type, image_url, created_at, updated_at, is_active " +
                           "FROM disks WHERE user_id = @userId AND disk_path = @diskPath AND is_active = TRUE LIMIT 1";

                using (var reader = await _dbService.ExecuteReaderAsync(query,
                    new MySqlParameter("@userId", userId),
                    new MySqlParameter("@diskPath", diskPath)))
                {
                    if (await reader.ReadAsync())
                    {
                        return new Disk
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            UserId = Convert.ToInt32(reader["user_id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString(),
                            DiskPath = reader["disk_path"].ToString(),
                            DiskType = reader["disk_type"].ToString(),
                            ImageUrl = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : null,
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            UpdatedAt = Convert.ToDateTime(reader["updated_at"]),
                            IsActive = Convert.ToBoolean(reader["is_active"])
                        };
                    }
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Updates a disk's snapshot info (description + timestamp)
        /// </summary>
        public async Task<bool> UpdateDiskSnapshotAsync(int diskId, string description = null)
        {
            try
            {
                var query = "UPDATE disks SET updated_at = NOW()";
                if (description != null)
                    query += ", description = @description";
                query += " WHERE id = @diskId";

                var parameters = new List<MySqlParameter> { new MySqlParameter("@diskId", diskId) };
                if (description != null)
                    parameters.Add(new MySqlParameter("@description", description));

                var result = await _dbService.ExecuteNonQueryAsync(query, parameters.ToArray());
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(bool success, string message)> UpdateDiskImageUrlAsync(int diskId, string imageUrl)
        {
            try
            {
                var query = "UPDATE disks SET image_url = @imageUrl, updated_at = NOW() WHERE id = @diskId";
                var result = await _dbService.ExecuteNonQueryAsync(query,
                    new MySqlParameter("@imageUrl", (object)imageUrl ?? DBNull.Value),
                    new MySqlParameter("@diskId", diskId));
                return result > 0 ? (true, "Image updated.") : (false, "Disk not found.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update image: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> UpdateDiskDetailsAsync(int diskId, string name = null, string description = null, string imageUrl = null)
        {
            try
            {
                var parts = new List<string>();
                var parameters = new List<MySqlParameter> { new MySqlParameter("@diskId", diskId) };

                if (name != null)
                {
                    parts.Add("name = @name");
                    parameters.Add(new MySqlParameter("@name", name));
                }
                if (description != null)
                {
                    parts.Add("description = @description");
                    parameters.Add(new MySqlParameter("@description", description));
                }
                if (imageUrl != null)
                {
                    parts.Add("image_url = @imageUrl");
                    parameters.Add(new MySqlParameter("@imageUrl", imageUrl));
                }

                if (parts.Count == 0) return (false, "Nothing to update.");

                parts.Add("updated_at = NOW()");
                var query = $"UPDATE disks SET {string.Join(", ", parts)} WHERE id = @diskId";
                var result = await _dbService.ExecuteNonQueryAsync(query, parameters.ToArray());
                return result > 0 ? (true, "Disk updated.") : (false, "Disk not found.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update disk: {ex.Message}");
            }
        }
    }
}
