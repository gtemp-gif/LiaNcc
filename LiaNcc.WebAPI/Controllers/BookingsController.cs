using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Requests;
using LiaNcc.Models.DTOs.Responses;
using LiaNcc.Models.Entities;
using LiaNcc.Models.Enums;
using LiaNcc.Repository.Interfaces;
using LiaNcc.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ILocalizedContentRepository _localizationRepository;
        private readonly LiaNcc.WebAPI.Helpers.ILocalizationResolver _resolver;
        private readonly IMailService _mailService;
        private readonly IApplicationLoggerService _logger;

        public BookingsController(
            IBookingRepository bookingRepository,
            ILocalizedContentRepository localizationRepository,
            LiaNcc.WebAPI.Helpers.ILocalizationResolver resolver,
            IMailService mailService,
            IApplicationLoggerService logger)
        {
            _bookingRepository = bookingRepository;
            _localizationRepository = localizationRepository;
            _resolver = resolver;
            _mailService = mailService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookings([FromQuery] BookingFilterRequest filter)
        {
            var bookings = await _bookingRepository.GetAllAsync(filter);
            return Ok(bookings.Select(MapToDetailDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDetailDto>> GetBooking(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return Ok(MapToDetailDto(booking));
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetByStatus(string status)
        {
            var bookings = await _bookingRepository.GetByStatusAsync(status);
            return Ok(bookings.Select(MapToDetailDto));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateBooking(BookingCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = new Booking
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                ServiceDate = request.ServiceDate,
                ServiceTypeId = request.ServiceTypeId,
                PassengerOptionId = request.PassengerOptionId,
                TourId = request.TourId,
                VehicleId = request.VehicleId,
                Message = request.Message,
                MaxSeats = request.MaxSeats,
                Status = "Pending",
                SourcePage = request.SourcePage,
                CreatedAt = DateTime.UtcNow
            };

            await _bookingRepository.CreateAsync(booking);

            // Reload booking with navigation properties for email notifications
            var detailedBooking = await _bookingRepository.GetByIdAsync(booking.Id);
            if (detailedBooking != null)
            {
                booking = detailedBooking;
            }

            await _logger.LogInformationAsync(
                area: "Bookings",
                action: "CreateBooking",
                message: "Booking created from public FE form",
                controller: "Bookings",
                entityName: "Booking",
                entityId: booking.Id,
                eventType: ApplicationEventType.Create.ToString(),
                additionalData: request
            );

            var emailSent = false;

            try
            {
                await _mailService.SendBookingNotificationAsync(booking, request);
                await _mailService.SendBookingCustomerConfirmationAsync(booking, request);

                emailSent = true;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    area: "Mail",
                    action: "BookingEmailFailed",
                    message: "Booking saved but email sending failed",
                    exception: ex,
                    controller: "Bookings",
                    entityName: "Booking",
                    entityId: booking.Id,
                    eventType: "Exception",
                    additionalData: request
                );
            }

            return Ok(new
            {
                success = true,
                booking,
                emailSent,
                message = emailSent
                    ? "Prenotazione inviata correttamente."
                    : "Prenotazione salvata, ma non è stato possibile inviare le email."
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(Guid id, Booking booking)
        {
            if (id != booking.Id) return BadRequest();
            await _bookingRepository.UpdateAsync(booking);
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] BookingStatusUpdateRequest request)
        {
            await _bookingRepository.UpdateStatusAsync(id, request.Status);
            await _logger.LogInformationAsync("Bookings", "UpdateStatus", $"Booking {id} status updated to {request.Status}", "Bookings", "Booking", id);

            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking != null)
            {
                try
                {
                    if (request.Status == "Confirmed")
                    {
                        await _mailService.SendBookingAcceptedAsync(booking);
                    }
                    else if (request.Status == "Cancelled")
                    {
                        await _mailService.SendBookingRejectedAsync(booking, request.Note ?? "Richiesta annullata dall'amministratore.");
                    }
                }
                catch (Exception ex)
                {
                    await _logger.LogErrorAsync("Bookings", "StatusUpdateEmailError", $"Errore invio email per cambio stato prenotazione {id}", ex);
                }
            }

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
        public async Task<ActionResult<IEnumerable<Service>>> GetServiceTypes([FromQuery] string? culture)
        {
            var types = await _bookingRepository.GetServiceTypesAsync();
            if (!string.IsNullOrEmpty(culture))
            {
                foreach (var type in types)
                {
                    var translations = await _localizationRepository.GetByEntityAsync("Service", type.Id, culture);
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

        private BookingDetailDto MapToDetailDto(Booking booking)
        {
            return new BookingDetailDto
            {
                Id = booking.Id,
                FullName = booking.FullName,
                Email = booking.Email,
                Phone = booking.Phone,
                ServiceDate = booking.ServiceDate,
                ServiceTypeId = booking.ServiceTypeId,
                ServiceTypeName = booking.ServiceType?.Name,
                ServiceTypeDescription = booking.ServiceType?.Description, // Can be added if needed, Service entity doesn't have it yet
                PassengerOptionId = booking.PassengerOptionId,
                PassengerOptionName = booking.PassengerOption?.Name,
                PassengerOptionDescription = null,
                TourId = booking.TourId,
                TourName = booking.Tour?.Name,
                TourDescription = booking.Tour?.Description,
                VehicleId = booking.VehicleId,
                VehicleName = booking.Vehicle?.Name,
                VehicleCategoryName = booking.Vehicle?.VehicleCategory?.Name,
                VehicleDescription = booking.Vehicle?.Description,
                MaxSeats = booking.MaxSeats,
                Message = booking.Message,
                Status = booking.Status,
                SourcePage = booking.SourcePage,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };
        }
    }
}
