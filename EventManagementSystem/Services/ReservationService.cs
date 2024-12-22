using System.Threading.Tasks;
using EventManagementSystem.Repositories;
using EventManagementSystem.Dto;
using EventManagementSystem.Models;

namespace EventManagementSystem.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ISeatRepository _seatRepository;

        public ReservationService(IReservationRepository reservationRepository, ISeatRepository seatRepository)
        {
            _reservationRepository = reservationRepository;
            _seatRepository = seatRepository;
        }

        public async Task<bool> CreateReservationAsync(Reservation reservation)
        {
            var isAvailable = await _seatRepository.CheckSeatAvailabilityAsync(reservation.SeatId);
            if (!isAvailable)
                return false;

            var result = await _reservationRepository.CreateReservationAsync(reservation);
            if (result)
            {
                await _seatRepository.UpdateSeatStatusAsync(reservation.SeatId, true, reservation.EndDate);
            }
            return result;
        }

        public async Task<bool> CancelReservationAsync(int reservationId)
        {
            var seatId = await _reservationRepository.GetSeatIdByReservationIdAsync(reservationId);

            if (seatId == null)
            {
                return false;
            }

            var result = await _reservationRepository.DeleteReservationAsync(reservationId);
            if (result)
            {
                await _seatRepository.UpdateSeatStatusAsync(seatId.Value, false, DateTime.MinValue);
            }
            return result;
        }

        public async Task<List<Reservation>> GetReservationsByEventAsync(int eventId)
        {
            var reservations = await _reservationRepository.GetReservationsByEventAsync(eventId);
            return reservations;
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            var reservations = await _reservationRepository.GetAllReservationsAsync();
            return reservations;
        }

        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            return reservation;
        }

        public async Task UpdateReservationAsync(Reservation reservation)
        {
            await _reservationRepository.UpdateReservationAsync(reservation);
        }
    }
}
