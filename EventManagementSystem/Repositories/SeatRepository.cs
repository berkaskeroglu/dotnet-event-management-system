using System;
using System.Data;
using System.Runtime.InteropServices;
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
            var query = "SELECT id, event_id, is_reserved, created_at, updated_at, reserved_until, category FROM seats WHERE id = @SeatId";

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
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                EventId = reader.GetInt32(reader.GetOrdinal("event_id")),
                                IsReserved = reader.GetBoolean(reader.GetOrdinal("is_reserved")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                                ReservedUntil = reader.GetDateTime(reader.GetOrdinal("reserved_until")),
                                Category = reader.GetInt32(reader.GetOrdinal("category"))
                            };
                        }
                    }
                }
            });

            return seat;
        }


        public async Task<bool> CheckSeatAvailabilityAsync(int seatId)
        {
            var query = "SELECT id, is_reserved, reserved_until FROM seats WHERE id = @SeatId";
            bool isAvailable = false;

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
                            int id = reader.GetInt32(reader.GetOrdinal("id"));
                            bool isReserved = reader.GetBoolean(reader.GetOrdinal("is_reserved"));
                            DateTime reservedUntil = reader.GetDateTime(reader.GetOrdinal("reserved_until"));

                            if (!isReserved || DateTime.Now > reservedUntil)
                            {
                                isAvailable = true;
                            }
                        }
                    }
                }
            });

            return isAvailable;
        }



        public async Task UpdateSeatStatusAsync(int seatId, bool isReserved, DateTime reservedUntil)
        {
            var query = "UPDATE seats SET is_reserved = @IsReserved, reserved_until = @ReservedUntil, updated_at = @UpdatedAt WHERE id = @SeatId";

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
            var query = "INSERT INTO seats (event_id, is_reserved, created_at, updated_at, reserved_until, category) " +
                        "VALUES (@EventId, @IsReserved, @CreatedAt, @UpdatedAt, @ReservedUntil, @Category)";

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();
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
                var query = "SELECT * FROM seats WHERE event_id = @EventId";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@EventId", eventId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            seats.Add(new Seat
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                EventId = reader.GetInt32(reader.GetOrdinal("event_id")),
                                IsReserved = reader.GetBoolean(reader.GetOrdinal("is_reserved")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                                ReservedUntil = reader.GetDateTime(reader.GetOrdinal("reserved_until")),
                                Category = reader.GetInt32(reader.GetOrdinal("category"))
                            });
                        }
                    }
                }
            });

            return seats; 
        }

    }
}
