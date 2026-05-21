using LiaNcc.FE.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.FE.Controllers
{
    public class ContactController : BaseController
    {
        private readonly IContactMessagesApiClient _contactApi;

        public ContactController(IContactMessagesApiClient contactApi)
        {
            _contactApi = contactApi;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(ContactMessage message)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _contactApi.CreateAsync(message);
                return Ok(new { success = true, message = "Messaggio inviato con successo." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Errore durante l'invio del messaggio." });
            }
        }
    }

    public class BookingsController : BaseController
    {
        private readonly IBookingsApiClient _bookingsApi;

        public BookingsController(IBookingsApiClient bookingsApi)
        {
            _bookingsApi = bookingsApi;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _bookingsApi.CreateAsync(booking);
                return Ok(new { success = true, message = "Prenotazione inviata con successo." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Errore durante l'invio della prenotazione." });
            }
        }
    }
}
