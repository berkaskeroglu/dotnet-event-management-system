namespace EventManagementSystem.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
