using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;

namespace LiaNcc.BO.Controllers
{
    public class LocalizationController : BaseController
    {
        private readonly ILanguagesApiClient _languagesApiClient;

        private readonly IApplicationLoggerService _logger;

        public LocalizationController(ILanguagesApiClient languagesApiClient, IApplicationLoggerService applicationLogger)
        {
            _languagesApiClient = languagesApiClient;
            _logger = applicationLogger;
        }

        public async Task<IActionResult> Index()
        {
            var languages = await _languagesApiClient.GetAllAsync();
            return View(languages);
        }
    }
}
