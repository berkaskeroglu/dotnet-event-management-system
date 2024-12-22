using EventManagementSystem.Models;

namespace EventManagementSystem.Repositories
{
    public interface IReservationRepository
    {
        Task<List<Reservation>> GetAllReservationsAsync();
        Task<Reservation> GetReservationByIdAsync(int id);
        Task<bool> CreateReservationAsync(Reservation reservation);
        Task UpdateReservationAsync(Reservation reservation);
        Task<List<Reservation>> GetReservationsByEventAsync(int eventId);
        Task<int?> GetSeatIdByReservationIdAsync(int reservationId);
        Task<bool> DeleteReservationAsync(int id);
    }

}
