namespace EventManagementSystem.Services
{
    public interface IReservationService
    {
        Task<bool> ReserveSeatAsync(int seatId, int userId);
        Task<bool> CancelReservationAsync(int seatId, int userId);
        Task<bool> CheckSeatAvailabilityAsync(int seatId);
        Task UpdateSeatStatusAsync(int seatId, bool isReserved);
    }

}
