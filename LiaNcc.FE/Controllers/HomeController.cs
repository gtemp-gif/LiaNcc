using System.Diagnostics;
using LiaNcc.FE.Models;
using LiaNcc.FE.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LiaNcc.FE.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISitePagesApiClient _sitePagesApi;
        private readonly IServicesApiClient _servicesApi;
        private readonly IVehiclesApiClient _vehiclesApi;
        private readonly IToursApiClient _toursApi;
        private readonly IPartnersApiClient _partnersApi;
        private readonly ICompanyApiClient _companyApi;

        public HomeController(
            ILogger<HomeController> logger,
            ISitePagesApiClient sitePagesApi,
            IServicesApiClient servicesApi,
            IVehiclesApiClient vehiclesApi,
            IToursApiClient toursApi,
            IPartnersApiClient partnersApi,
            ICompanyApiClient companyApi)
        {
            _logger = logger;
            _sitePagesApi = sitePagesApi;
            _servicesApi = servicesApi;
            _vehiclesApi = vehiclesApi;
            _toursApi = toursApi;
            _partnersApi = partnersApi;
            _companyApi = companyApi;
        }

        public async Task<IActionResult> Index()
        {
            var culture = CurrentCulture;
            var model = new HomeViewModel();
            try { model.Page = await _sitePagesApi.GetFullBySlugAsync("home", culture); } catch (Exception ex) { _logger.LogError(ex, "Error loading CMS page"); }
            try { model.FeaturedServices = await _servicesApi.GetFeaturedAsync(culture); } catch (Exception ex) { _logger.LogError(ex, "Error loading services"); }
            try { model.FeaturedVehicles = await _vehiclesApi.GetFeaturedAsync(culture); } catch (Exception ex) { _logger.LogError(ex, "Error loading vehicles"); }
            try { model.FeaturedTours = await _toursApi.GetFeaturedAsync(culture); } catch (Exception ex) { _logger.LogError(ex, "Error loading tours"); }
            try { model.Partners = await _partnersApi.GetActiveAsync(); } catch (Exception ex) { _logger.LogError(ex, "Error loading partners"); }
            try { model.Company = await _companyApi.GetCompanyProfileAsync(); } catch (Exception ex) { _logger.LogError(ex, "Error loading company profile"); }

            return View(model);
        }

        public async Task<IActionResult> Fleet()
        {
            var culture = CurrentCulture;
            var model = new FleetViewModel();
            try { model.Vehicles = await _vehiclesApi.GetActiveAsync(culture); } catch (Exception ex) { _logger.LogError(ex, "Error loading vehicles"); }
            return View(model);
        }

        public async Task<IActionResult> Tours()
        {
            var culture = CurrentCulture;
            var model = new ToursViewModel();
            try { model.Tours = await _toursApi.GetActiveAsync(culture); } catch (Exception ex) { _logger.LogError(ex, "Error loading tours"); }
            return View(model);
        }

        public async Task<IActionResult> TourDetail(string slug)
        {
            var culture = CurrentCulture;
            Tour? tour = null;
            try { tour = await _toursApi.GetDetailBySlugAsync(slug, culture); } catch (Exception ex) { _logger.LogError(ex, "Error loading tour detail"); }

            if (tour == null) return NotFound();

            var model = new TourDetailViewModel
            {
                Tour = tour
            };
            return View(model);
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
