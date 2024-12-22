namespace EventManagementSystem.Dto
{
    public class ReservationDto
    {
        public int SeatId { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
    }

}
