using EventManagementSystem.Models;
using EventManagementSystem.Data;
using Npgsql;
using System.Data;

namespace EventManagementSystem.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public RoleRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            const string query = "SELECT * FROM roles";
            var roles = new List<Role>();

            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                await using (var command = new NpgsqlCommand(query, connection))
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        roles.Add(new Role
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name"))
                        });
                    }
                }
            }

            return roles;
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM roles WHERE id = @Id";

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
                            return new Role
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<Role> CreateAsync(Role role)
        {
            const string query = @"
                INSERT INTO roles (name)
                VALUES (@Name)
                RETURNING id;";

            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", role.Name);

                    role.Id = (int)await command.ExecuteScalarAsync();
                }
            }

            return role;
        }

        public async Task UpdateAsync(Role role)
        {
            const string query = "UPDATE roles SET name = @Name WHERE id = @Id";

            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                await using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", role.Id);
                    command.Parameters.AddWithValue("@Name", role.Name);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            const string query = "DELETE FROM roles WHERE id = @Id";

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
