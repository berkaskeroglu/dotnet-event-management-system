using System.Numerics;

namespace EventManagementSystem.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public bool IsReserved { get; set; }
        public DateTime ReservedUntil { get; set; }
        public int Category {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
