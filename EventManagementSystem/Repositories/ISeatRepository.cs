using EventManagementSystem.Models;

namespace EventManagementSystem.Repositories
{
    public interface ISeatRepository
    {
        Task CreateSeatAsync(Seat seat);
        Task<Seat> GetSeatAsync(int seatId);
        Task<bool> CheckSeatAvailabilityAsync(int seatId);
        Task<List<Seat>> GetSeatsByEventAsync(int eventId);
        Task UpdateSeatStatusAsync(int seatId, bool isReserved, DateTime reservedUntil);
    }

}
