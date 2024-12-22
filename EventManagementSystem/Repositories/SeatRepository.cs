using System;
using System.Data;
using System.Threading.Tasks;
using EventManagementSystem.Data;
using EventManagementSystem.Models;
using Npgsql;

namespace EventManagementSystem.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly DatabaseConnectionService _dbConnectionService;
        private readonly QueuedDatabaseExecutor _executor;

        public SeatRepository(DatabaseConnectionService dbConnectionService, QueuedDatabaseExecutor executor)
        {
            _dbConnectionService = dbConnectionService;
            _executor = executor;
        }

        public async Task<Seat> GetSeatAsync(int seatId)
        {
            Seat seat = null;
            var query = "SELECT * FROM Seats WHERE Id = @SeatId";

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SeatId", seatId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            seat = new Seat
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                                IsReserved = reader.GetBoolean(reader.GetOrdinal("IsReserved")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                ReservedUntil = reader.GetDateTime(reader.GetOrdinal("ReservedUntil")),
                                Category = reader.GetInt32(reader.GetOrdinal("Category"))
                            };
                        }
                    }
                }
            });

            return seat;
        }


        public async Task<bool> CheckSeatAvailabilityAsync(int seatId)
        {
            var query = "SELECT IsReserved FROM Seats WHERE Id = @SeatId";
            bool isAvailable = false;

            await _executor.ExecuteAsync(async () =>
            {
                var connection =  _dbConnectionService.GetConnection(); 
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SeatId", seatId);
                    var result = await cmd.ExecuteScalarAsync();
                    isAvailable = result != DBNull.Value && !(bool)result;
                }
            });

            return isAvailable;
        }


        public async Task UpdateSeatStatusAsync(int seatId, bool isReserved, DateTime reservedUntil)
        {
            var query = "UPDATE Seats SET IsReserved = @IsReserved, ReservedUntil = @ReservedUntil, UpdatedAt = @UpdatedAt WHERE Id = @SeatId";

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection(); 
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SeatId", seatId);
                    cmd.Parameters.AddWithValue("@IsReserved", isReserved);
                    cmd.Parameters.AddWithValue("@ReservedUntil", reservedUntil);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }


        public async Task CreateSeatAsync(Seat seat)
        {
            var query = "INSERT INTO Seats (EventId, IsReserved, CreatedAt, UpdatedAt, ReservedUntil, Category) " +
                        "VALUES (@EventId, @IsReserved, @CreatedAt, @UpdatedAt, @ReservedUntil, @Category)";

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection(); // Halihazırda açılmış bağlantıyı alıyoruz.
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@EventId", seat.EventId);
                    cmd.Parameters.AddWithValue("@IsReserved", seat.IsReserved);
                    cmd.Parameters.AddWithValue("@CreatedAt", seat.CreatedAt);
                    cmd.Parameters.AddWithValue("@UpdatedAt", seat.UpdatedAt);
                    cmd.Parameters.AddWithValue("@ReservedUntil", seat.ReservedUntil);
                    cmd.Parameters.AddWithValue("@Category", seat.Category);

                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }

        public async Task<List<Seat>> GetSeatsByEventAsync(int eventId)
        {
            List<Seat> seats = new List<Seat>();

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();
                var query = "SELECT * FROM Seats WHERE EventId = @EventId";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@EventId", eventId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            seats.Add(new Seat
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                                IsReserved = reader.GetBoolean(reader.GetOrdinal("IsReserved")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                ReservedUntil = reader.GetDateTime(reader.GetOrdinal("ReservedUntil")),
                                Category = reader.GetInt32(reader.GetOrdinal("Category"))
                            });
                        }
                    }
                }
            });

            return seats;  // Koltukları döndürüyoruz.
        }

    }
}
