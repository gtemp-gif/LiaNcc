using System.Diagnostics;
using LiaNcc.FE.Models;
using LiaNcc.FE.Models.ViewModels;
using LiaNcc.FE.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.FE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILiaNccApiClient _apiClient;

        public HomeController(ILogger<HomeController> logger, ILiaNccApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        private string GetCurrentCulture()
        {
            var culture = Request.Query["culture"].ToString();
            if (string.IsNullOrEmpty(culture))
            {
                culture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            }
            return culture;
        }

        public async Task<IActionResult> Index()
        {
            var culture = GetCurrentCulture();
            var viewModel = new HomePageViewModel
            {
                CmsPage = await _apiClient.GetSitePageBySlugAsync("home", culture),
                FeaturedServices = (await _apiClient.GetServicesAsync(culture)).Where(s => s.IsFeatured && s.IsActive).OrderBy(s => s.SortOrder).ThenBy(s => s.Name),
                FeaturedVehicles = (await _apiClient.GetVehiclesAsync(culture)).Where(v => v.IsFeatured && v.IsActive).OrderBy(v => v.SortOrder).ThenBy(v => v.Name),
                FeaturedTours = (await _apiClient.GetToursAsync(culture)).Where(t => t.IsFeatured && t.IsActive).OrderBy(t => t.SortOrder).ThenBy(t => t.Name),
                Partners = await _apiClient.GetPartnersAsync(true),
                CompanyProfile = await _apiClient.GetCompanyProfileAsync(culture)
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Fleet()
        {
            var culture = GetCurrentCulture();
            var viewModel = new FleetPageViewModel
            {
                CmsPage = await _apiClient.GetSitePageBySlugAsync("fleet", culture),
                Vehicles = (await _apiClient.GetVehiclesAsync(culture)).Where(v => v.IsActive).OrderBy(v => v.SortOrder).ThenBy(v => v.Name)
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Tours()
        {
            var culture = GetCurrentCulture();
            var viewModel = new ToursPageViewModel
            {
                CmsPage = await _apiClient.GetSitePageBySlugAsync("tours", culture),
                Tours = (await _apiClient.GetToursAsync(culture)).Where(t => t.IsActive).OrderBy(t => t.SortOrder).ThenBy(t => t.Name)
            };
            return View(viewModel);
        }

        public async Task<IActionResult> TourDetail(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var culture = GetCurrentCulture();
            var tour = await _apiClient.GetTourBySlugAsync(slug, culture);

            if (tour == null || !tour.IsActive)
            {
                return NotFound();
            }

            var viewModel = new TourDetailViewModel
            {
                Tour = tour
            };
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
