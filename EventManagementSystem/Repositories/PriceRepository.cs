using EventManagementSystem.Data;
using EventManagementSystem.Models;
using Microsoft.AspNetCore.Connections;
using Npgsql;

namespace EventManagementSystem.Repositories
{
    public class PriceRepository : IPriceRepository
    {
        private readonly DatabaseConnectionService _dbConnectionService;
        private readonly QueuedDatabaseExecutor _executor;
        public PriceRepository(DatabaseConnectionService dbConnectionService, QueuedDatabaseExecutor executor)
        {
            _dbConnectionService = dbConnectionService;
            _executor = executor;
        }

        public async Task<List<Price>> GetAllPricesAsync()
        {
            var prices = new List<Price>();

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();
                var query = "SELECT * FROM Prices";
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        prices.Add(new Price
                        {
                            EventId = reader.GetInt32(0),
                            Category = reader.GetInt32(1),
                            Value = reader.GetDecimal(2)
                        });
                    }
                }
            });


            return prices;
        }

        public async Task<Price> AddPrice(int eventId, int category)
        {
            Price addedPrice = null;

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                var query = "INSERT INTO Prices (EventId, Category, Price) VALUES (@EventId, @Category, 0.0) RETURNING EventId, Category, Price";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@EventId", eventId);
                    cmd.Parameters.AddWithValue("@Category", category);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            addedPrice = new Price
                            {
                                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                                Category = reader.GetInt32(reader.GetOrdinal("Category")),
                                Value = reader.GetDecimal(reader.GetOrdinal("Price"))
                            };
                        }
                    }
                }
            });

            if (addedPrice == null)
            {
                throw new Exception("Failed to add price.");
            }

            return addedPrice;
        }


        public async Task<List<Price>> GetPriceByEventAsync(int eventId)
        {
            var prices = new List<Price>();

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

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
            });

            return prices;
        }


        public async Task<Price> GetPriceByEventAndCategoryAsync(int eventId, int category)
        {
            Price price = null;

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                var query = "SELECT EventId, Category, Price FROM Prices WHERE EventId = @EventId AND Category = @Category";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@EventId", eventId);
                    cmd.Parameters.AddWithValue("@Category", category);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            price = new Price
                            {
                                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                                Category = reader.GetInt32(reader.GetOrdinal("Category")),
                                Value = reader.GetDecimal(reader.GetOrdinal("Price"))
                            };
                        }
                    }
                }
            });

            if (price == null)
            {
                throw new KeyNotFoundException("Price not found for the specified EventId and Category.");
            }

            return price;
        }


        public async Task UpdatePriceAsync(int eventId, int category, decimal newPrice)
        {
            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                var query = "UPDATE Prices SET Price = @NewPrice WHERE EventId = @EventId AND Category = @Category";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@NewPrice", newPrice);
                    cmd.Parameters.AddWithValue("@EventId", eventId);
                    cmd.Parameters.AddWithValue("@Category", category);

                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }

    }

}
