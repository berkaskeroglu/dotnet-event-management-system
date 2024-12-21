using EventManagementSystem.Data;
using EventManagementSystem.Models;
using Microsoft.AspNetCore.Connections;
using Npgsql;

namespace EventManagementSystem.Repositories
{
    public class PriceRepository : IPriceRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public PriceRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<Price>> GetAllPricesAsync()
        {
            var prices = new List<Price>();

            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                var query = "SELECT EventId, Category, Price FROM Prices";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            prices.Add(new Price
                            {
                                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                                Category = reader.GetInt32(reader.GetOrdinal("Category")),
                                Value = reader.GetDecimal(reader.GetOrdinal("Price"))
                            });
                        }
                    }
                }
            }

            return prices;
        }

        public async Task<Price> AddPrice(int eventId, int category)
        {
            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                var query = "INSERT INTO Prices (EventId, Category, Price) VALUES (@EventId, @Category, 0.0) RETURNING EventId, Category, Price";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@EventId", eventId);
                    cmd.Parameters.AddWithValue("@Category", category);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Price
                            {
                                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                                Category = reader.GetInt32(reader.GetOrdinal("Category")),
                                Value = reader.GetDecimal(reader.GetOrdinal("Price"))
                            };
                        }
                    }
                }
            }

            throw new Exception("Failed to add price.");
        }

        public async Task<List<Price>> GetPriceByEventAsync(int eventId)
        {
            var prices = new List<Price>();

            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                var query = "SELECT EventId, Category, Price FROM Prices WHERE EventId = @EventId";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@EventId", eventId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            prices.Add(new Price
                            {
                                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                                Category = reader.GetInt32(reader.GetOrdinal("Category")),
                                Value = reader.GetDecimal(reader.GetOrdinal("Price"))
                            });
                        }
                    }
                }
            }

            return prices;
        }

        public async Task<Price> GetPriceByEventAndCategoryAsync(int eventId, int category)
        {
            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                var query = "SELECT EventId, Category, Price FROM Prices WHERE EventId = @EventId AND Category = @Category";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@EventId", eventId);
                    cmd.Parameters.AddWithValue("@Category", category);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Price
                            {
                                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                                Category = reader.GetInt32(reader.GetOrdinal("Category")),
                                Value = reader.GetDecimal(reader.GetOrdinal("Price"))
                            };
                        }
                    }
                }
            }

            throw new KeyNotFoundException("Price not found for the specified EventId and Category.");
        }

        public async Task UpdatePriceAsync(int eventId, int category, decimal newPrice)
        {
            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                var query = "UPDATE Prices SET Price = @NewPrice WHERE EventId = @EventId AND Category = @Category";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@NewPrice", newPrice);
                    cmd.Parameters.AddWithValue("@EventId", eventId);
                    cmd.Parameters.AddWithValue("@Category", category);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }

}
