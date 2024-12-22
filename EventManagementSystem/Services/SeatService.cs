using EventManagementSystem.Models;
using EventManagementSystem.Repositories;

namespace EventManagementSystem.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;

        public SeatService(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        public async Task<Seat> GetSeatByIdAsync(int seatId)
        {
            return await _seatRepository.GetSeatAsync(seatId);
        }

        public async Task CreateSeatAsync(Seat seat)
        {
            await _seatRepository.CreateSeatAsync(seat);
        }

        public async Task<bool> UpdateSeatStatusAsync(int seatId, bool isReserved, DateTime reservedUntil)
        {
            var seat = await _seatRepository.GetSeatAsync(seatId);
            if (seat == null)
            {
                return false;
            }

            await _seatRepository.UpdateSeatStatusAsync(seatId, isReserved, reservedUntil);
            return true;
        }

        public async Task<List<Seat>> GetSeatsByEventAsync(int eventId)
        {
            return await _seatRepository.GetSeatsByEventAsync(eventId);
        }

        public async Task<bool> CheckSeatAvailabilityAsync(int seatId)
        {
            var seat = await _seatRepository.GetSeatAsync(seatId);
            if (seat == null)
            {
                return false;
            }

            return !seat.IsReserved;
        }
    }
}
