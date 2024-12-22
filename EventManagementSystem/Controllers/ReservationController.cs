using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Services;
using EventManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        // GET: api/reservations
        [HttpGet]
        public async Task<ActionResult<List<Reservation>>> GetAllReservations()
        {
            var reservations = await _reservationService.GetAllReservationsAsync();
            if (reservations == null || reservations.Count == 0)
            {
                return NotFound("No reservations found.");
            }
            return Ok(reservations);
        }

        // GET: api/reservations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservationById(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound($"Reservation with ID {id} not found.");
            }
            return Ok(reservation);
        }

        // POST: api/reservations
        [HttpPost]
        public async Task<ActionResult<Reservation>> CreateReservation([FromBody] Reservation reservation)
        {
            if (reservation == null)
            {
                return BadRequest("Reservation data is invalid.");
            }

            var result = await _reservationService.CreateReservationAsync(reservation);
            if (result)
            {
                return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
            }

            return BadRequest("Failed to create reservation.");
        }

        // PUT: api/reservations
        [HttpPut]
        public async Task<ActionResult> UpdateReservation([FromBody] Reservation reservation)
        {
            if (reservation == null)
            {
                return BadRequest("Reservation data is invalid.");
            }

            await _reservationService.UpdateReservationAsync(reservation);
            return NoContent();
        }

        // GET: api/reservations/event/{eventId}
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<List<Reservation>>> GetReservationsByEvent(int eventId)
        {
            var reservations = await _reservationService.GetReservationsByEventAsync(eventId);
            if (reservations == null || reservations.Count == 0)
            {
                return NotFound($"No reservations found for event with ID {eventId}.");
            }
            return Ok(reservations);
        }

        // DELETE: api/reservations/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> CancelReservation(int id)
        {
            var result = await _reservationService.CancelReservationAsync(id);
            if (result)
            {
                return NoContent();
            }

            return NotFound($"Reservation with ID {id} not found or could not be canceled.");
        }
    }
}
