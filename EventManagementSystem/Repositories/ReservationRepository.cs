using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Hubs;
using Npgsql;
using Microsoft.AspNetCore.SignalR;

namespace EventManagementSystem.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly DatabaseConnectionService _dbConnectionService;
        private readonly QueuedDatabaseExecutor _executor;
        private readonly IHubContext<ReservationHub> _hubContext;

        public ReservationRepository(DatabaseConnectionService dbConnectionService, QueuedDatabaseExecutor executor, IHubContext<ReservationHub> hubContext)
        {
            _dbConnectionService = dbConnectionService;
            _executor = executor;
            _hubContext = hubContext;
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            List<Reservation> reservations = new List<Reservation>();

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                var query = "SELECT * FROM reservations";

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var reservation = new Reservation
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
                            EventId = reader.GetInt32(reader.GetOrdinal("event_id")),
                            Price = reader.GetInt32(reader.GetOrdinal("price")),
                            SeatId = reader.GetInt32(reader.GetOrdinal("seat_id")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("start_date")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("end_date")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            Status = reader.GetInt32(reader.GetOrdinal("status")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                        };
                        reservations.Add(reservation);
                    }
                }
            });

            return reservations;
        }



        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            Reservation reservation = null;

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                var query = "SELECT id, user_id, event_id, seat_id, start_date, end_date, price, status, created_at, updated_at FROM reservations WHERE Id = @Id";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            reservation = new Reservation
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
                                EventId = reader.GetInt32(reader.GetOrdinal("event_id")),
                                Price = reader.GetInt32(reader.GetOrdinal("price")),
                                SeatId = reader.GetInt32(reader.GetOrdinal("seat_id")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("start_date")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("end_date")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                Status = reader.GetInt32(reader.GetOrdinal("status")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                            };
                        }
                    }
                }
            });

            return reservation;
        }



        public async Task<bool> CreateReservationAsync(Reservation reservation)
        {
            var result = false;

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                var query = "INSERT INTO reservations (user_id, event_id, seat_id, start_date, end_date, price, status, category, created_at) " +
                            "VALUES (@UserId, @EventId, @SeatId, @StartDate, @EndDate, @Price, @Status, @Category, @CreatedAt)";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", reservation.UserId);
                    cmd.Parameters.AddWithValue("@EventId", reservation.EventId);
                    cmd.Parameters.AddWithValue("@SeatId", reservation.SeatId);
                    cmd.Parameters.AddWithValue("@Price", reservation.Price);
                    cmd.Parameters.AddWithValue("@StartDate", reservation.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", reservation.EndDate);
                    cmd.Parameters.AddWithValue("@Status", reservation.Status);
                    cmd.Parameters.AddWithValue("@Category", reservation.Category);
                    cmd.Parameters.AddWithValue("@CreatedAt", reservation.CreatedAt);


                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    result = rowsAffected > 0;
                }
            });

            return result;
        }





        public async Task UpdateReservationAsync(Reservation reservation)
        {
            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();

                var query = "UPDATE reservations SET user_id = @UserId, event_id = @EventId, seat_id = @SeatId, price = @Price, start_date = @StartDate, end_date = @EndDate, status = @Status, category = @Category, updated_at = @UpdatedAt WHERE Id = @Id";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", reservation.UserId);
                    cmd.Parameters.AddWithValue("@EventId", reservation.EventId);
                    cmd.Parameters.AddWithValue("@SeatId", reservation.SeatId);
                    cmd.Parameters.AddWithValue("@Price", reservation.Price);
                    cmd.Parameters.AddWithValue("@StartDate", reservation.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", reservation.EndDate);
                    cmd.Parameters.AddWithValue("@Status", reservation.Status);
                    cmd.Parameters.AddWithValue("@Category", reservation.Category);
                    cmd.Parameters.AddWithValue("@UpdatedAt", reservation.UpdatedAt);
                    cmd.Parameters.AddWithValue("@Id", reservation.Id);

                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }



        public async Task<bool> DeleteReservationAsync(int reservationId)
        {
            var result = false;

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();
                var query = "DELETE FROM reservations WHERE Id = @Id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", reservationId);
                    var affectedRows = await command.ExecuteNonQueryAsync();
                    result = affectedRows > 0; 
                }
            });

            return result;
        }



        public async Task<int?> GetSeatIdByReservationIdAsync(int reservationId)
        {
            int? seatId = null;

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();
                var query = "SELECT seat_id FROM reservations WHERE Id = @ReservationId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReservationId", reservationId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            seatId = reader.GetInt32(reader.GetOrdinal("seat_id"));
                        }
                    }
                }
            });

            return seatId;
        }

        public async Task<List<Reservation>> GetReservationsByEventAsync(int eventId)
        {
            var reservations = new List<Reservation>();

            await _executor.ExecuteAsync(async () =>
            {
                var connection = _dbConnectionService.GetConnection();
                var query = "SELECT * FROM reservations WHERE event_id = @EventId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EventId", eventId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var reservation = new Reservation
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
                                EventId = reader.GetInt32(reader.GetOrdinal("event_id")),
                                Price = reader.GetInt32(reader.GetOrdinal("price")),
                                SeatId = reader.GetInt32(reader.GetOrdinal("seat_id")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("start_date")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("end_date")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                Status = reader.GetInt32(reader.GetOrdinal("status")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                            };
                            reservations.Add(reservation);
                        }
                    }
                }
            });

            return reservations;
        }




    }
}
