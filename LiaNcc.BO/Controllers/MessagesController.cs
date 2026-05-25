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
    }
}
