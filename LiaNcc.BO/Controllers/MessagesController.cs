using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.DTOs.Requests;
using LiaNcc.BO.Helpers;

namespace LiaNcc.BO.Controllers
{
    public class MessagesController : BaseController
    {
        private readonly IContactMessagesApiClient _contactMessagesApiClient;

        private readonly IApplicationLoggerService _logger;

        public MessagesController(IContactMessagesApiClient contactMessagesApiClient, IApplicationLoggerService applicationLogger)
        {
            _contactMessagesApiClient = contactMessagesApiClient;
            _logger = applicationLogger;
        }

        public async Task<IActionResult> Index(ContactMessageFilterRequest filter)
        {
            // Convert local Rome dates to UTC for API query
            if (filter.FromDate.HasValue) filter.FromDate = filter.FromDate.Value.ToUtcFromRome();
            if (filter.ToDate.HasValue) filter.ToDate = filter.ToDate.Value.Date.AddDays(1).AddTicks(-1).ToUtcFromRome();

            var messages = await _contactMessagesApiClient.GetAllAsync(filter);

            // Re-convert to Rome for display in filter inputs
            if (filter.FromDate.HasValue) filter.FromDate = filter.FromDate.Value.ToRomeTime();
            if (filter.ToDate.HasValue) filter.ToDate = filter.ToDate.Value.ToRomeTime();

            ViewBag.Filter = filter;
            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _contactMessagesApiClient.MarkAsReadAsync(id);
            await _logger.LogInformationAsync("Messages", "MarkAsRead", $"Messaggio {id} segnato come letto", "Messages", "ContactMessage", id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _contactMessagesApiClient.DeleteAsync(id);
            await _logger.LogInformationAsync("Messages", "Delete", $"Messaggio {id} eliminato", "Messages", "ContactMessage", id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(Guid id, string subject, string body, List<IFormFile>? attachments)
        {
            await _contactMessagesApiClient.ReplyAsync(id, subject, body, attachments);
            await _logger.LogInformationAsync("Messages", "Reply", $"Risposta inviata per il messaggio {id}", "Messages", "ContactMessage", id);
            return RedirectToAction(nameof(Index));
        }
    }
}
