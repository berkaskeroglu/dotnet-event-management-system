using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Dto
{
    public class UpdateSeatStatusRequest
    {
        [Required]
        public int SeatId { get; set; }

        [Required]
        public bool IsReserved { get; set; }
    }
}
