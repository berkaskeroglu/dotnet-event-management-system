using EventManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Dto;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    // POST: api/Reservation/reserve
    [HttpPost("reserve")]
    public async Task<IActionResult> ReserveSeat([FromBody] ReserveSeatRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _reservationService.ReserveSeatAsync(request.SeatId, request.UserId);
        if (result)
            return Ok("Seat reserved successfully.");

        return BadRequest("Failed to reserve the seat. It may already be reserved.");
    }

    // POST: api/Reservation/cancel
    [HttpPost("cancel")]
    public async Task<IActionResult> CancelReservation([FromBody] CancelReservationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _reservationService.CancelReservationAsync(request.SeatId, request.UserId);
        if (result)
            return Ok("Reservation canceled successfully.");

        return BadRequest("Failed to cancel the reservation. Ensure the seat is reserved by the user.");
    }

    // GET: api/Reservation/availability/{seatId}
    [HttpGet("availability/{seatId}")]
    public async Task<IActionResult> CheckSeatAvailability(int seatId)
    {
        var isAvailable = await _reservationService.CheckSeatAvailabilityAsync(seatId);
        return Ok(new { SeatId = seatId, IsAvailable = isAvailable });
    }

    // POST: api/Reservation/update-status
    [HttpPost("update-status")]
    public async Task<IActionResult> UpdateSeatStatus([FromBody] UpdateSeatStatusRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _reservationService.UpdateSeatStatusAsync(request.SeatId, request.IsReserved);
        return Ok("Seat status updated successfully.");
    }
}
