using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

namespace TagMyDrive.Services
{
    public class MembershipService
    {
        private readonly DatabaseService _dbService;

        public class Membership
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int MaxDisks { get; set; }
            public decimal PriceMonthly { get; set; }

            public override string ToString()
            {
                return $"{Name} - {(PriceMonthly > 0 ? $"${PriceMonthly}/mo" : "Free")}";
            }
        }

        public MembershipService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<List<Membership>> GetAllMembershipsAsync()
        {
            var memberships = new List<Membership>();
            try
            {
                var query = "SELECT id, name, description, max_disks, price_monthly FROM memberships ORDER BY max_disks";

                using (var reader = await _dbService.ExecuteReaderAsync(query))
                {
                    while (await reader.ReadAsync())
                    {
                        memberships.Add(new Membership
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString(),
                            MaxDisks = Convert.ToInt32(reader["max_disks"]),
                            PriceMonthly = Convert.ToDecimal(reader["price_monthly"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get memberships: {ex.Message}", ex);
            }

            return memberships;
        }

        public async Task<Membership> GetMembershipByIdAsync(int membershipId)
        {
            try
            {
                var query = "SELECT id, name, description, max_disks, price_monthly FROM memberships WHERE id = @id";

                using (var reader = await _dbService.ExecuteReaderAsync(query,
                    new MySqlParameter("@id", membershipId)))
                {
                    if (await reader.ReadAsync())
                    {
                        return new Membership
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString(),
                            MaxDisks = Convert.ToInt32(reader["max_disks"]),
                            PriceMonthly = Convert.ToDecimal(reader["price_monthly"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get membership: {ex.Message}", ex);
            }

            return null;
        }

        public async Task<(bool success, string message)> UpgradeMembershipAsync(int userId, int newMembershipId)
        {
            try
            {
                var membershipQuery = "SELECT id FROM memberships WHERE id = @membershipId";
                var exists = await _dbService.ExecuteScalarAsync(membershipQuery,
                    new MySqlParameter("@membershipId", newMembershipId));

                if (exists == null)
                    return (false, "Membership not found.");

                var updateQuery = "UPDATE users SET membership_id = @membershipId WHERE id = @userId";
                var result = await _dbService.ExecuteNonQueryAsync(updateQuery,
                    new MySqlParameter("@membershipId", newMembershipId),
                    new MySqlParameter("@userId", userId));

                return (result > 0, result > 0 ? "Membership upgraded successfully." : "Failed to upgrade.");
            }
            catch (Exception ex)
            {
                return (false, $"Upgrade failed: {ex.Message}");
            }
        }

        public async Task<Membership> GetUserMembershipAsync(int userId)
        {
            try
            {
                var query = "SELECT m.id, m.name, m.description, m.max_disks, m.price_monthly " +
                           "FROM memberships m " +
                           "INNER JOIN users u ON u.membership_id = m.id " +
                           "WHERE u.id = @userId";

                using (var reader = await _dbService.ExecuteReaderAsync(query,
                    new MySqlParameter("@userId", userId)))
                {
                    if (await reader.ReadAsync())
                    {
                        return new Membership
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString(),
                            MaxDisks = Convert.ToInt32(reader["max_disks"]),
                            PriceMonthly = Convert.ToDecimal(reader["price_monthly"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get user membership: {ex.Message}", ex);
            }

            return null;
        }

        public async Task<(bool isAtLimit, int currentCount, int maxAllowed)> IsUserAtDiskLimitAsync(int userId)
        {
            try
            {
                var query = "SELECT COUNT(d.id) as disk_count, m.max_disks " +
                           "FROM users u " +
                           "INNER JOIN memberships m ON u.membership_id = m.id " +
                           "LEFT JOIN disks d ON d.user_id = u.id AND d.is_active = TRUE " +
                           "WHERE u.id = @userId " +
                           "GROUP BY u.id";

                using (var reader = await _dbService.ExecuteReaderAsync(query,
                    new MySqlParameter("@userId", userId)))
                {
                    if (await reader.ReadAsync())
                    {
                        var currentCount = Convert.ToInt32(reader["disk_count"]);
                        var maxAllowed = Convert.ToInt32(reader["max_disks"]);
                        return (currentCount >= maxAllowed, currentCount, maxAllowed);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check disk limit: {ex.Message}", ex);
            }

            return (false, 0, 0);
        }
    }
}
