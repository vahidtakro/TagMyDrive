using System;
using System.Threading.Tasks;
using MySqlConnector;

namespace TagMyDrive.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Gets a new database connection
        /// </summary>
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        /// <summary>
        /// Tests the database connection
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    await conn.OpenAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Executes a non-query command
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(string query, params MySqlParameter[] parameters)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        return await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Database query error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a query and returns the first column of the first row
        /// </summary>
        public async Task<object> ExecuteScalarAsync(string query, params MySqlParameter[] parameters)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        return await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Database query error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a query and returns a data reader
        /// </summary>
        public async Task<MySqlDataReader> ExecuteReaderAsync(string query, params MySqlParameter[] parameters)
        {
            try
            {
                var conn = GetConnection();
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return (MySqlDataReader)await cmd.ExecuteReaderAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Database query error: {ex.Message}", ex);
            }
        }
    }
}
