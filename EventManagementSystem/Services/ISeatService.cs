using EventManagementSystem.Models;

namespace EventManagementSystem.Services
{
    public interface ISeatService
    {
        Task<Seat> GetSeatByIdAsync(int seatId);
        Task CreateSeatAsync(Seat seat);
        Task<bool> UpdateSeatStatusAsync(int seatId, bool isReserved, DateTime reservedUntil);
        Task<List<Seat>> GetSeatsByEventAsync(int eventId);
        Task<bool> CheckSeatAvailabilityAsync(int seatId);
    }
}
