using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Dto
{
    public class AddPriceRequest
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int Category { get; set; }
    }
}
