using System;
using System.Threading.Tasks;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.BO.Controllers
{
    public class CMSController : BaseController
    {
        private readonly ISitePagesApiClient _sitePagesApiClient;

        public CMSController(ISitePagesApiClient sitePagesApiClient)
        {
            _sitePagesApiClient = sitePagesApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var pages = await _sitePagesApiClient.GetAllAsync();
            return View(pages);
        }
    }
}
