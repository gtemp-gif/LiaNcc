using LiaNcc.FE.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.FE.Controllers
{
    public class ContactController : BaseController
    {
        private readonly IContactMessagesApiClient _contactApi;
        private readonly IApplicationLoggerService _logger;

        public ContactController(IContactMessagesApiClient contactApi, IApplicationLoggerService applicationLogger)
        {
            _contactApi = contactApi;
            _logger = applicationLogger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(LiaNcc.Models.DTOs.Requests.ContactMessageCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _contactApi.CreateAsync(request);
                await _logger.LogInformationAsync("Contact", "SendMessage", $"Message from {request.FullName} sent", "Contact", "ContactMessage");
                return Ok(new { success = true, message = "Messaggio inviato con successo." });
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("Contact", "SendMessage", "Error sending contact message", ex, 500, "Contact", "ContactMessage");
                return StatusCode(500, new { success = false, message = "Errore durante l'invio del messaggio." });
            }
        }
    }

    public class BookingsController : BaseController
    {
        private readonly IBookingsApiClient _bookingsApi;
        private readonly IApplicationLoggerService _logger;

        public BookingsController(IBookingsApiClient bookingsApi, IApplicationLoggerService applicationLogger)
        {
            _bookingsApi = bookingsApi;
            _logger = applicationLogger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LiaNcc.Models.DTOs.Requests.BookingCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _bookingsApi.CreateAsync(request);
                await _logger.LogInformationAsync("Bookings", "CreateBooking", $"Booking from {request.FullName} sent", "Bookings", "Booking");
                return Ok(new { success = true, message = "Prenotazione inviata con successo." });
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("Bookings", "CreateBooking", "Error sending booking request", ex, 500, "Bookings", "Booking");
                return StatusCode(500, new { success = false, message = "Errore durante l'invio della prenotazione." });
            }
        }
    }
}
