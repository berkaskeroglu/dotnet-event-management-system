namespace EventManagementSystem.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }

        public int EventId { get; set; }
        public int SeatId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }
        public int Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Category { get; set; }

     
    }
}
