namespace EventManagementSystem.Dto
{
    public class SeatDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public bool IsReserved { get; set; }
        public DateTime ReservedUntil { get; set; }
        public int Category { get; set; }
    }

}
