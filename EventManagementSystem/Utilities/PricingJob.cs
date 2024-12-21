using EventManagementSystem.Data;
using Npgsql;
using Quartz;
using System;
using System.Data;
using System.Threading.Tasks;
using EventManagementSystem.Models;

public class DynamicPricingJob : IJob
{
    private readonly DatabaseConnection _dbConnection;

    public DynamicPricingJob(DatabaseConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
        {
            await connection.OpenAsync();

            var pricesQuery = "SELECT EventId, Category, Price FROM Prices";
            var prices = new List<Price>();

            using (var cmd = new NpgsqlCommand(pricesQuery, connection))
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

            foreach (var price in prices)
            {
                var seatsQuery = @"
                    SELECT COUNT(*) AS TotalSeats, 
                           SUM(CASE WHEN IsReserved = FALSE THEN 1 ELSE 0 END) AS AvailableSeats
                    FROM Seats
                    WHERE EventId = @EventId AND Category = @Category";

                int totalSeats = 0;
                int availableSeats = 0;

                using (var cmd = new NpgsqlCommand(seatsQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@EventId", price.EventId);
                    cmd.Parameters.AddWithValue("@Category", price.Category);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            totalSeats = reader.GetInt32(reader.GetOrdinal("TotalSeats"));
                            availableSeats = reader.GetInt32(reader.GetOrdinal("AvailableSeats"));
                        }
                    }
                }

                var newPrice = await CalculateDynamicPriceAsync(price, totalSeats, availableSeats);

                var updateQuery = "UPDATE Prices SET Price = @NewPrice WHERE EventId = @EventId AND Category = @Category";

                using (var cmd = new NpgsqlCommand(updateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@NewPrice", newPrice);
                    cmd.Parameters.AddWithValue("@EventId", price.EventId);
                    cmd.Parameters.AddWithValue("@Category", price.Category);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }

    private async Task<decimal> CalculateDynamicPriceAsync(Price price, int totalSeats, int availableSeats)
    {
        decimal seatRatio = availableSeats / totalSeats;

        if (seatRatio <= 0.10m)
        {
            return price.Value * 2.0m;
        }
        else if (seatRatio <= 0.25m)
        {
            return price.Value * 1.75m;
        }
        else if (seatRatio <= 0.50m)
        {
            return price.Value * 1.25m;
        }
        else
        {
            return price.Value;
        }
    }
}


