using EventManagementSystem.Dto;
using EventManagementSystem.Models;

namespace EventManagementSystem.Services
{
    public interface IReservationService
    {
        Task<List<Reservation>> GetAllReservationsAsync();
        Task<Reservation> GetReservationByIdAsync(int id);
        Task<bool> CreateReservationAsync(Reservation reservation);
        Task UpdateReservationAsync(Reservation reservation);
        Task<List<Reservation>> GetReservationsByEventAsync(int eventId);
        Task<bool> CancelReservationAsync(int reservationId);
    }

}
