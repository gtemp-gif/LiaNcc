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

        public LocalizationController(ILanguagesApiClient languagesApiClient)
        {
            _languagesApiClient = languagesApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var languages = await _languagesApiClient.GetAllAsync();
            return View(languages);
        }
    }
}
