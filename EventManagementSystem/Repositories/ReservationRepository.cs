using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Hubs;
using Npgsql;
using Microsoft.AspNetCore.SignalR;

namespace EventManagementSystem.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly DatabaseConnection _dbConnection;
        private readonly IHubContext<ReservationHub> _hubContext;

        public ReservationRepository(DatabaseConnection dbConnection, IHubContext<ReservationHub> hubContext)
        {
            _dbConnection = dbConnection;
            _hubContext = hubContext;
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            var reservations = new List<Reservation>();
            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

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
            }
            return reservations;
        }


        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            Reservation reservation = null;

            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                var query = "SELECT Id, UserId, EventId, SeatId, ReservationDate FROM Reservations WHERE Id = @Id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
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
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                ReservedUntil = reader.GetDateTime(reader.GetOrdinal("ReservedUntil")),
                                Status = (ReservationStatus)reader.GetInt32(reader.GetOrdinal("Status"))
                            };
                        }
                    }
                }
            }

            return reservation;
        }


        public async Task CreateReservationAsync(Reservation reservation)
        {
            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                var query = "INSERT INTO Reservations (UserId, EventId, SeatId, Price, Status, ReservedUntil, CreatedAt) VALUES (@UserId, @EventId, @SeatId, @Price, @Status, @ReservedUntil, @CreatedAt)";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", reservation.UserId);
                    cmd.Parameters.AddWithValue("@EventId", reservation.EventId);
                    cmd.Parameters.AddWithValue("@SeatId", reservation.SeatId);
                    cmd.Parameters.AddWithValue("@Price", reservation.Price);
                    cmd.Parameters.AddWithValue("@Status", reservation.Status); //???????????
                    cmd.Parameters.AddWithValue("@ReservedUntil", reservation.ReservedUntil); //???????????
                    cmd.Parameters.AddWithValue("@CreatedAt", reservation.CreatedAt);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task UpdateReservationAsync(Reservation reservation)
        {
            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();
                var query = "UPDATE Reservations SET UserId = @UserId, EventId = @EventId, SeatId = @SeatId, Price = @Price, ReservedUntil = @ReservedUntil, UpdatedAt = @UpdatedAt WHERE Id = @Id";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    // Parametreleri ekleyelim
                    cmd.Parameters.AddWithValue("UserId", reservation.UserId);
                    cmd.Parameters.AddWithValue("EventId", reservation.EventId);
                    cmd.Parameters.AddWithValue("SeatId", reservation.SeatId);
                    cmd.Parameters.AddWithValue("@Price", reservation.Price);
                    cmd.Parameters.AddWithValue("@ReservedUntil", reservation.ReservedUntil);
                    cmd.Parameters.AddWithValue("UpdatedAt", reservation.UpdatedAt);
                    cmd.Parameters.AddWithValue("Id", reservation.Id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task DeleteReservationAsync(int id)
        {
            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();
                var query = "DELETE FROM Reservations WHERE Id = @Id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> IsSeatAvailableAsync(int seatId)
        {
            bool isAvailable = false;
            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();
                var query = "SELECT IsReserved, ReservedUntil FROM Seats WHERE Id = @SeatId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("SeatId", seatId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var isReserved = reader.GetBoolean(reader.GetOrdinal("IsReserved"));
                            var reservedUntil = reader.IsDBNull(reader.GetOrdinal("ReservedUntil")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ReservedUntil"));

                            if (!isReserved || (reservedUntil.HasValue && reservedUntil.Value < DateTime.UtcNow))
                            {
                                isAvailable = true;
                            }
                        }
                    }
                }
            }
            return isAvailable;
        }


        public async Task ReserveSeatForTemporaryPeriodAsync(int seatId, int durationInSeconds)
        {
            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();
                var query = @"
                        UPDATE Seats
                        SET IsReserved = TRUE, ReservedUntil = CURRENT_TIMESTAMP + INTERVAL @DurationInSeconds SECOND
                        WHERE Id = @SeatId AND (IsReserved = FALSE OR ReservedUntil < CURRENT_TIMESTAMP)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SeatId", seatId);
                    command.Parameters.AddWithValue("@DurationInSeconds", durationInSeconds);

                    var affectedRows = await command.ExecuteNonQueryAsync();

                    if (affectedRows == 0)
                    {
                        throw new InvalidOperationException("The seat is already reserved or unavailable.");
                    }
                }
            }
            await _hubContext.Clients.All.SendAsync("ReceiveSeatUpdate", seatId, true);
        }


        public async Task CancelTemporaryReservationAsync(int seatId)
        {
            using (var connection = _dbConnection.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                var query = "UPDATE Seats SET IsReserved = FALSE, ReservedUntil = NULL WHERE Id = @SeatId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("SeatId", seatId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }



    }
}
