using System.Threading.Tasks;
using EventManagementSystem.Repositories;

namespace EventManagementSystem.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationService(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<bool> ReserveSeatAsync(int seatId, int userId)
        {
            // Koltuğun uygunluğunu kontrol et
            var isAvailable = await _reservationRepository.CheckSeatAvailabilityAsync(seatId);
            if (!isAvailable)
                return false;

            // Koltuğu rezerve et
            var result = await _reservationRepository.ReserveSeatAsync(seatId, userId);
            return result;
        }

        public async Task<bool> CancelReservationAsync(int seatId, int userId)
        {
            // Rezervasyonu iptal et
            var result = await _reservationRepository.CancelReservationAsync(seatId, userId);
            return result;
        }

        public async Task<bool> CheckSeatAvailabilityAsync(int seatId)
        {
            // Koltuk uygunluğunu kontrol et
            var isAvailable = await _reservationRepository.CheckSeatAvailabilityAsync(seatId);
            return isAvailable;
        }

        public async Task UpdateSeatStatusAsync(int seatId, bool isReserved)
        {
            // Koltuk durumunu güncelle
            await _reservationRepository.UpdateSeatStatusAsync(seatId, isReserved);
        }
    }
}
