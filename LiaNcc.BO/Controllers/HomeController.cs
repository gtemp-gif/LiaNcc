using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using LiaNcc.BO.Models;

namespace LiaNcc.BO.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IDashboardApiClient _dashboardApiClient;

        public HomeController(IDashboardApiClient dashboardApiClient)
        {
            _dashboardApiClient = dashboardApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var summary = await _dashboardApiClient.GetSummaryAsync();
            if (summary == null)
            {
                summary = new LiaNcc.Models.DTOs.Dashboard.DashboardSummaryDto();
                TempData["ErrorMessage"] = "Impossibile caricare i dati della dashboard.";
            }

            return View(summary);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
