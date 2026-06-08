using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;

namespace LiaNcc.BO.Controllers
{
    public class BookingsController : BaseController
    {
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly LiaNcc.BO.Services.Interfaces.IApplicationLoggerService _logger;

        public BookingsController(IBookingsApiClient bookingsApiClient, LiaNcc.BO.Services.Interfaces.IApplicationLoggerService logger)
        {
            _bookingsApiClient = bookingsApiClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingsApiClient.GetAllAsync();
            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid id, string status, string? note = null)
        {
            await _bookingsApiClient.UpdateStatusAsync(id, status, note);
            await _logger.LogInformationAsync("Bookings", "UpdateStatus", $"Stato prenotazione {id} aggiornato a {status}. Nota: {note}", "Bookings", "Booking", id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _bookingsApiClient.DeleteAsync(id);
            await _logger.LogInformationAsync("Bookings", "Delete", $"Prenotazione {id} eliminata", "Bookings", "Booking", id);
            return RedirectToAction(nameof(Index));
        }
    }
}
