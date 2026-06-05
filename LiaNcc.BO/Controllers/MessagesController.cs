using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;

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

        public async Task<IActionResult> Index()
        {
            var messages = await _contactMessagesApiClient.GetAllAsync();
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
    }
}
