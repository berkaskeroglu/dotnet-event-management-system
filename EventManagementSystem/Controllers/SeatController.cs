using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Services;
using EventManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        // GET: api/seats/{seatId}
        [HttpGet("{seatId}")]
        public async Task<ActionResult<Seat>> GetSeatById(int seatId)
        {
            var seat = await _seatService.GetSeatByIdAsync(seatId);
            if (seat == null)
            {
                return NotFound($"Seat with ID {seatId} not found.");
            }
            return Ok(seat);
        }

        // POST: api/seats
        [HttpPost]
        public async Task<ActionResult<Seat>> CreateSeat([FromBody] Seat seat)
        {
            if (seat == null)
            {
                return BadRequest("Seat data is invalid.");
            }

            await _seatService.CreateSeatAsync(seat);
            return CreatedAtAction(nameof(GetSeatById), new { seatId = seat.Id }, seat);
        }

        // PUT: api/seats/{seatId}
        [HttpPut("{seatId}")]
        public async Task<ActionResult> UpdateSeatStatus(int seatId, [FromBody] Seat seat)
        {
            if (seat == null || seat.Id != seatId)
            {
                return BadRequest("Seat data is invalid or ID mismatch.");
            }

            var result = await _seatService.UpdateSeatStatusAsync(seatId, seat.IsReserved, seat.ReservedUntil);
            if (!result)
            {
                return NotFound($"Seat with ID {seatId} not found.");
            }

            return NoContent();
        }

        // GET: api/seats/event/{eventId}
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<List<Seat>>> GetSeatsByEvent(int eventId)
        {
            var seats = await _seatService.GetSeatsByEventAsync(eventId);
            if (seats == null || seats.Count == 0)
            {
                return NotFound($"No seats found for event with ID {eventId}.");
            }
            return Ok(seats);
        }

        // GET: api/seats/check/{seatId}
        [HttpGet("checkAvailability/{seatId}")]
        public async Task<ActionResult<bool>> CheckSeatAvailability(int seatId)
        {
            var isAvailable = await _seatService.CheckSeatAvailabilityAsync(seatId);
            return Ok(isAvailable);
        }
    }
}


// UPDATE SEAT GELECEK. EKSİK.