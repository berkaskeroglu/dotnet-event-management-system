using EventManagementSystem.Models;
using EventManagementSystem.Data;
using Npgsql;
using System.Data;

namespace EventManagementSystem.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DatabaseConnectionService _dbConnectionService;
        private readonly QueuedDatabaseExecutor _executor;

        public RoleRepository(DatabaseConnectionService dbConnectionService, QueuedDatabaseExecutor executor)
        {
            _dbConnectionService = dbConnectionService;
            _executor = executor;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            const string query = "SELECT * FROM roles";
            var roles = new List<Role>();

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
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
            });

            return roles;
        }


        public async Task<Role> GetByIdAsync(int id)
        {
            Role role = null;

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                const string query = "SELECT * FROM roles WHERE id = @Id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            role = new Role
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name"))
                            };
                        }
                    }
                }
            });

            return role;
        }


        public async Task<Role> CreateAsync(Role role)
        {
            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                const string query = @"
                        INSERT INTO roles (name)
                        VALUES (@Name)
                        RETURNING id;";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", role.Name);

                    role.Id = (int)await command.ExecuteScalarAsync();
                }
            });

            return role;
        }


        public async Task UpdateAsync(Role role)
        {
            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                const string query = "UPDATE roles SET name = @Name WHERE id = @Id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", role.Id);
                    command.Parameters.AddWithValue("@Name", role.Name);

                    await command.ExecuteNonQueryAsync();
                }
            });
        }


        public async Task DeleteAsync(int id)
        {
            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                const string query = "DELETE FROM roles WHERE id = @Id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await command.ExecuteNonQueryAsync();
                }
            });
        }

    }
}
