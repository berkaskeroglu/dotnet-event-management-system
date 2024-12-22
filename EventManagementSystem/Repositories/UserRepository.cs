using EventManagementSystem.Data;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace EventManagementSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseConnectionService _dbConnectionService;
        private readonly QueuedDatabaseExecutor _executor;

        public UserRepository(DatabaseConnectionService dbConnectionService, QueuedDatabaseExecutor executor)
        {
            _dbConnectionService = dbConnectionService;
            _executor = executor;
        }

        public async Task CreateAsync(User user)
        {
            await _executor.ExecuteAsync(async () =>
            {
                const string query = @"
                    INSERT INTO users 
                    (id, username, email, password_hash, password_salt, role_id, created_at, updated_at, name)
                    VALUES 
                    (@Id, @Username, @Email, @PasswordHash, @PasswordSalt, @RoleId, @CreatedAt, @UpdatedAt, @Name);";

                var connection = _dbConnectionService.GetConnection();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt);
                    command.Parameters.AddWithValue("@RoleId", user.RoleId);
                    command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                    command.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt);
                    command.Parameters.AddWithValue("@Name", user.Name);

                    await command.ExecuteNonQueryAsync();
                }
            });
        }


        public async Task<IEnumerable<User>> GetAllAsync()
        {
            IEnumerable<User> users = null;

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();
                const string query = @"SELECT * FROM users";

                var userList = new List<User>();

                await using (var command = new NpgsqlCommand(query, connection))
                {
                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            userList.Add(new User
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("id")),
                                Username = reader.GetString(reader.GetOrdinal("username")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                                PasswordSalt = reader.GetString(reader.GetOrdinal("password_salt")),
                                RoleId = reader.GetInt32(reader.GetOrdinal("role_id")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                            });
                        }
                    }
                }

                users = userList;
            });

            if (users == null)
            {
                throw new Exception("Failed to fetch users.");
            }

            return users;
        }


        public async Task<User> GetByEmailAsync(string email)
        {
            User user = null;

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                const string query = @"SELECT * FROM users WHERE email = @Email;";

                await using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new User
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("id")),
                                Username = reader.GetString(reader.GetOrdinal("username")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                                PasswordSalt = reader.GetString(reader.GetOrdinal("password_salt")),
                                RoleId = reader.GetInt32(reader.GetOrdinal("role_id")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                            };
                        }
                    }
                }
            });

            return user;
        }


        public async Task<User> GetByIdAsync(int id)
        {
            User user = null;

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();
                const string query = @"SELECT * FROM users WHERE id = @Id;";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new User
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("id")),
                                Username = reader.GetString(reader.GetOrdinal("username")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                                PasswordSalt = reader.GetString(reader.GetOrdinal("password_salt")),
                                RoleId = reader.GetInt32(reader.GetOrdinal("role_id")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                            };
                        }
                    }
                }
            });

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return user;
        }


        public async Task<User> GetByUsernameAsync(string username)
        {
            User user = null;

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                const string query = @"SELECT * FROM users WHERE username = @Username;";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new User
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("id")),
                                Username = reader.GetString(reader.GetOrdinal("username")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                                PasswordSalt = reader.GetString(reader.GetOrdinal("password_salt")),
                                RoleId = reader.GetInt32(reader.GetOrdinal("role_id")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                            };
                        }
                    }
                }
            });

            return user;
        }


        public async Task UpdateAsync(User user)
        {
            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                const string query = @"
                        UPDATE users
                        SET username = @Username, email = @Email, updated_at = @UpdatedAt
                        WHERE id = @Id;";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt);

                    await command.ExecuteNonQueryAsync();
                }
            });
        }


        public async Task DeleteAsync(int id)
        {
            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                const string query = @"DELETE FROM users WHERE id = @Id;";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await command.ExecuteNonQueryAsync();
                }
            });
        }
    }

}
