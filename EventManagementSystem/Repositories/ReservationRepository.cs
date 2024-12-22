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

                var query = "SELECT * FROM Reservations";

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var reservation = new Reservation
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                            EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                            Price = reader.GetInt32(reader.GetOrdinal("Price")),
                            SeatId = reader.GetInt32(reader.GetOrdinal("SeatId")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                            ReservedUntil = reader.GetDateTime(reader.GetOrdinal("ReservedUntil")),
                            Status = (ReservationStatus)reader.GetInt32(reader.GetOrdinal("Status"))
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

                var query = "SELECT Id, UserId, EventId, SeatId, ReservationDate, Price, ReservedUntil, Status FROM Reservations WHERE Id = @Id";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            reservation = new Reservation
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                                Price = reader.GetInt32(reader.GetOrdinal("Price")),
                                SeatId = reader.GetInt32(reader.GetOrdinal("SeatId")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("ReservationDate")),
                                ReservedUntil = reader.IsDBNull(reader.GetOrdinal("ReservedUntil")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ReservedUntil")),
                                Status = (ReservationStatus)reader.GetInt32(reader.GetOrdinal("Status"))
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

                var query = "INSERT INTO Reservations (UserId, EventId, SeatId, Price, Status, ReservedUntil, CreatedAt) " +
                            "VALUES (@UserId, @EventId, @SeatId, @Price, @Status, @ReservedUntil, @CreatedAt)";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", reservation.UserId);
                    cmd.Parameters.AddWithValue("@EventId", reservation.EventId);
                    cmd.Parameters.AddWithValue("@SeatId", reservation.SeatId);
                    cmd.Parameters.AddWithValue("@Price", reservation.Price);
                    cmd.Parameters.AddWithValue("@Status", reservation.Status);
                    cmd.Parameters.AddWithValue("@ReservedUntil", reservation.ReservedUntil);
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

                var query = "UPDATE Reservations SET UserId = @UserId, EventId = @EventId, SeatId = @SeatId, Price = @Price, ReservedUntil = @ReservedUntil, UpdatedAt = @UpdatedAt WHERE Id = @Id";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("UserId", reservation.UserId);
                    cmd.Parameters.AddWithValue("EventId", reservation.EventId);
                    cmd.Parameters.AddWithValue("SeatId", reservation.SeatId);
                    cmd.Parameters.AddWithValue("Price", reservation.Price);
                    cmd.Parameters.AddWithValue("ReservedUntil", reservation.ReservedUntil);
                    cmd.Parameters.AddWithValue("UpdatedAt", reservation.UpdatedAt);
                    cmd.Parameters.AddWithValue("Id", reservation.Id);

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
                var query = "DELETE FROM Reservations WHERE Id = @Id";

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
                var query = "SELECT SeatId FROM Reservations WHERE Id = @ReservationId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReservationId", reservationId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            seatId = reader.GetInt32(reader.GetOrdinal("SeatId"));
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
                var query = "SELECT * FROM Reservations WHERE EventId = @EventId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EventId", eventId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var reservation = new Reservation
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                                SeatId = reader.GetInt32(reader.GetOrdinal("SeatId")),
                                Category = reader.GetInt32(reader.GetOrdinal("Category")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Status = (ReservationStatus)reader.GetInt32(reader.GetOrdinal("Status")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
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
