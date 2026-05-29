using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Requests;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using LiaNcc.WebAPI.Services;
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
        private readonly ILocalizedContentRepository _localizationRepository;
        private readonly LiaNcc.WebAPI.Helpers.ILocalizationResolver _resolver;
        private readonly IApplicationLoggerService _logger;

        public BookingsController(
            IBookingRepository bookingRepository,
            ILocalizedContentRepository localizationRepository,
            LiaNcc.WebAPI.Helpers.ILocalizationResolver resolver,
            IApplicationLoggerService logger)
        {
            _bookingRepository = bookingRepository;
            _localizationRepository = localizationRepository;
            _resolver = resolver;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return Ok(await _bookingRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetByStatus(string status)
        {
            return Ok(await _bookingRepository.GetByStatusAsync(status));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(CreateBookingRequest request)
        {
            var booking = new Booking
            {
                FullName = request.FullName,
                Email = request.Email,
                ServiceDate = request.ServiceDate,
                ServiceType = null, // Will depend on ServiceTypeId in a complete DTO mapping
                Message = request.Message,
                Status = "Pending"
            };

            await _bookingRepository.CreateAsync(booking);
            await _logger.LogInformationAsync("Bookings", "CreateBooking", $"Booking created for {booking.FullName}", "Bookings", "Booking", booking.Id);
            return Ok(booking);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(Guid id, Booking booking)
        {
            if (id != booking.Id) return BadRequest();
            await _bookingRepository.UpdateAsync(booking);
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
        {
            await _bookingRepository.UpdateStatusAsync(id, status);
            await _logger.LogInformationAsync("Bookings", "UpdateStatus", $"Booking {id} status updated to {status}", "Bookings", "Booking", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            await _bookingRepository.DeleteAsync(id);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("service-types")]
        public async Task<ActionResult<IEnumerable<BookingServiceType>>> GetServiceTypes([FromQuery] string? culture)
        {
            var types = await _bookingRepository.GetServiceTypesAsync();
            if (!string.IsNullOrEmpty(culture))
            {
                foreach (var type in types)
                {
                    var translations = await _localizationRepository.GetByEntityAsync("BookingServiceType", type.Id, culture);
                    type.Name = _resolver.Resolve(translations, "Name", type.Name, culture);
                }
            }
            return Ok(types);
        }

        [AllowAnonymous]
        [HttpGet("passenger-options")]
        public async Task<ActionResult<IEnumerable<BookingPassengerOption>>> GetPassengerOptions([FromQuery] string? culture)
        {
            var options = await _bookingRepository.GetPassengerOptionsAsync();
            if (!string.IsNullOrEmpty(culture))
            {
                foreach (var option in options)
                {
                    var translations = await _localizationRepository.GetByEntityAsync("BookingPassengerOption", option.Id, culture);
                    option.Name = _resolver.Resolve(translations, "Name", option.Name, culture);
                }
            }
            return Ok(options);
        }
    }
}
