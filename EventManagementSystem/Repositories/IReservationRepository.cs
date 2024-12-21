using EventManagementSystem.Models;

namespace EventManagementSystem.Repositories
{
    public interface IReservationRepository
    {
        Task<List<Reservation>> GetAllReservationsAsync();
        Task<Reservation> GetReservationByIdAsync(int id);
        Task CreateReservationAsync(Reservation reservation);
        Task UpdateReservationAsync(Reservation reservation);
        Task DeleteReservationAsync(int id);
        Task<bool> IsSeatAvailableAsync(int seatId);
        Task ReserveSeatForTemporaryPeriodAsync(int seatId, int durationInSeconds);
    }

}
