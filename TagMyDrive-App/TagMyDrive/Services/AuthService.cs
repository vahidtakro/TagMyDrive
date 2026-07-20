using System;
using System.Threading.Tasks;
using MySqlConnector;
using BCrypt.Net;

namespace TagMyDrive.Services
{
    public class AuthService
    {
        private readonly DatabaseService _dbService;

        public class User
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int MembershipId { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsActive { get; set; }
        }

        public AuthService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        public async Task<(bool success, string message, int? userId)> RegisterAsync(string username, string email, string password, string firstName = "", string lastName = "")
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return (false, "Username, email, and password are required.", null);
                }

                // Check if user already exists
                var query = "SELECT COUNT(*) FROM users WHERE username = @username OR email = @email";
                var result = await _dbService.ExecuteScalarAsync(query,
                    new MySqlParameter("@username", username),
                    new MySqlParameter("@email", email));

                if (result != null && Convert.ToInt64(result) > 0)
                {
                    return (false, "Username or email already exists.", null);
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                // Insert new user (default to Free membership)
                var insertQuery = "INSERT INTO users (username, email, password_hash, first_name, last_name, membership_id) " +
                                 "VALUES (@username, @email, @passwordHash, @firstName, @lastName, 1)";

                await _dbService.ExecuteNonQueryAsync(insertQuery,
                    new MySqlParameter("@username", username),
                    new MySqlParameter("@email", email),
                    new MySqlParameter("@passwordHash", passwordHash),
                    new MySqlParameter("@firstName", firstName ?? ""),
                    new MySqlParameter("@lastName", lastName ?? ""));

                // Get the new user ID
                var getIdQuery = "SELECT id FROM users WHERE username = @username";
                var userId = Convert.ToInt32(await _dbService.ExecuteScalarAsync(getIdQuery,
                    new MySqlParameter("@username", username)));

                return (true, "Registration successful.", userId);
            }
            catch (Exception ex)
            {
                return (false, $"Registration failed: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Authenticates a user and returns their info
        /// </summary>
        public async Task<(bool success, string message, User user)> LoginAsync(string username, string password)
        {
            try
            {
                // Get user by username or email
                var query = "SELECT id, username, email, password_hash, first_name, last_name, membership_id, created_at, is_active " +
                           "FROM users WHERE (username = @username OR email = @username) AND is_active = TRUE";

                using (var reader = await _dbService.ExecuteReaderAsync(query,
                    new MySqlParameter("@username", username)))
                {
                    if (await reader.ReadAsync())
                    {
                        var passwordHash = reader["password_hash"].ToString();

                        // Verify password
                        if (!BCrypt.Net.BCrypt.Verify(password, passwordHash))
                        {
                            return (false, "Invalid password.", null);
                        }

                        var user = new User
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Username = reader["username"].ToString(),
                            Email = reader["email"].ToString(),
                            FirstName = reader["first_name"].ToString(),
                            LastName = reader["last_name"].ToString(),
                            MembershipId = Convert.ToInt32(reader["membership_id"]),
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            IsActive = Convert.ToBoolean(reader["is_active"])
                        };

                        return (true, "Login successful.", user);
                    }

                    return (false, "User not found.", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Login failed: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Gets user information by ID
        /// </summary>
        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                var query = "SELECT id, username, email, first_name, last_name, membership_id, created_at, is_active " +
                           "FROM users WHERE id = @userId";

                using (var reader = await _dbService.ExecuteReaderAsync(query,
                    new MySqlParameter("@userId", userId)))
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Username = reader["username"].ToString(),
                            Email = reader["email"].ToString(),
                            FirstName = reader["first_name"].ToString(),
                            LastName = reader["last_name"].ToString(),
                            MembershipId = Convert.ToInt32(reader["membership_id"]),
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            IsActive = Convert.ToBoolean(reader["is_active"])
                        };
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Updates user password
        /// </summary>
        public async Task<(bool success, string message)> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            try
            {
                // Verify old password first
                var query = "SELECT password_hash FROM users WHERE id = @userId";
                var currentHash = Convert.ToString(await _dbService.ExecuteScalarAsync(query,
                    new MySqlParameter("@userId", userId)));

                if (currentHash == null || !BCrypt.Net.BCrypt.Verify(oldPassword, currentHash))
                {
                    return (false, "Current password is incorrect.");
                }

                // Hash new password and update
                var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                var updateQuery = "UPDATE users SET password_hash = @newHash WHERE id = @userId";

                await _dbService.ExecuteNonQueryAsync(updateQuery,
                    new MySqlParameter("@newHash", newHash),
                    new MySqlParameter("@userId", userId));

                return (true, "Password changed successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Password change failed: {ex.Message}");
            }
        }
    }
}
