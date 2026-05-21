using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using System.Threading.Tasks;
using System.Linq;

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
    }
}
