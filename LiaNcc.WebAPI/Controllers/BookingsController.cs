using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Requests;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingsController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return Ok(bookings);
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        // Public endpoint to allow users/guests to create bookings
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(CreateBookingRequest request)
        {
            var booking = new Booking
            {
                FullName = request.FullName,
                Email = request.Email,
                ServiceDate = request.ServiceDate,
                ServiceType = request.ServiceType,
                Message = request.Message
            };

            await _bookingRepository.CreateAsync(booking);
            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(Guid id, Booking booking)
        {
            if (id != booking.Id) return BadRequest();

            var existing = await _bookingRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _bookingRepository.UpdateAsync(booking);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            var existing = await _bookingRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _bookingRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
