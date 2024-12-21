using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Dto
{
    public class UpdatePriceRequest
    {
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal NewPrice { get; set; }
    }
}
