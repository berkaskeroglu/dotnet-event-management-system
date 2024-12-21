using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Dto
{
    public class ReserveSeatRequest
    {
        [Required]
        public int SeatId { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
