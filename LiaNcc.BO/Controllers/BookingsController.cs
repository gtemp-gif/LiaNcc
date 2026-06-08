using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.DTOs.Requests;
using LiaNcc.BO.Helpers;

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

        public async Task<IActionResult> Index(BookingFilterRequest filter)
        {
            // Convert local Rome dates to UTC for API query
            if (filter.FromServiceDate.HasValue) filter.FromServiceDate = filter.FromServiceDate.Value.ToUtcFromRome();
            if (filter.ToServiceDate.HasValue) filter.ToServiceDate = filter.ToServiceDate.Value.Date.AddDays(1).AddTicks(-1).ToUtcFromRome();
            if (filter.FromCreatedAt.HasValue) filter.FromCreatedAt = filter.FromCreatedAt.Value.ToUtcFromRome();
            if (filter.ToCreatedAt.HasValue) filter.ToCreatedAt = filter.ToCreatedAt.Value.Date.AddDays(1).AddTicks(-1).ToUtcFromRome();

            var bookings = await _bookingsApiClient.GetAllAsync(filter);

            // Re-convert to Rome for display in filter inputs
            if (filter.FromServiceDate.HasValue) filter.FromServiceDate = filter.FromServiceDate.Value.ToRomeTime();
            if (filter.ToServiceDate.HasValue) filter.ToServiceDate = filter.ToServiceDate.Value.ToRomeTime();
            if (filter.FromCreatedAt.HasValue) filter.FromCreatedAt = filter.FromCreatedAt.Value.ToRomeTime();
            if (filter.ToCreatedAt.HasValue) filter.ToCreatedAt = filter.ToCreatedAt.Value.ToRomeTime();

            ViewBag.Filter = filter;
            ViewBag.ServiceTypes = await _bookingsApiClient.GetServiceTypesAsync();
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
