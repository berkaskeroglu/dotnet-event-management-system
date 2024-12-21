using EventManagementSystem.Data;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace EventManagementSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly DatabaseConnection _dbConnection;

        public UserRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task CreateAsync(User user)
        {
            const string query = @"
            INSERT INTO users 
            (id, username, email, password_hash, password_salt, role_id, created_at, updated_at, name)
            VALUES 
            (@Id, @Username, @Email, @PasswordHash, @PasswordSalt, @RoleId, @CreatedAt, @UpdatedAt, @Name);";

            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                await using (var command = new NpgsqlCommand(query, connection))
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
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            const string query = @"SELECT * FROM users";

            var users = new List<User>();

            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                await using (var command = new NpgsqlCommand(query, connection))
                {
                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
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
            }

            return users;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            const string query = @"SELECT * FROM users WHERE email = @Email;";

            User user = null;

            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

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
            }

            return user;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            const string query = @"SELECT * FROM users WHERE id = @Id;";

            User user = null;

            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                await using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

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
            }

            return user;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            const string query = @"SELECT * FROM users WHERE username = @Username;";

            User user = null;

            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                await using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

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
            }

            return user;
        }

        public async Task UpdateAsync(User user)
        {
            const string query = @"
                UPDATE users
                SET username = @Username, email = @Email, updated_at = @UpdatedAt
                WHERE id = @Id;";

            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                await using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            const string query = @"DELETE FROM users WHERE id = @Id;";

            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                await using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }

}
