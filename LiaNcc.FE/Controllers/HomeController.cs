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
        private readonly IApplicationLoggerService _appLogger;
        private readonly ISitePagesApiClient _sitePagesApi;
        private readonly IServicesApiClient _servicesApi;
        private readonly IVehiclesApiClient _vehiclesApi;
        private readonly IToursApiClient _toursApi;
        private readonly IPartnersApiClient _partnersApi;
        private readonly ICompanyApiClient _companyApi;
        private readonly IBookingsApiClient _bookingsApi;

        public HomeController(
            ILogger<HomeController> logger,
            IApplicationLoggerService applicationLogger,
            ISitePagesApiClient sitePagesApi,
            IServicesApiClient servicesApi,
            IVehiclesApiClient vehiclesApi,
            IToursApiClient toursApi,
            IPartnersApiClient partnersApi,
            ICompanyApiClient companyApi,
            IBookingsApiClient bookingsApi)
        {
            _logger = logger;
            _appLogger = applicationLogger;
            _sitePagesApi = sitePagesApi;
            _servicesApi = servicesApi;
            _vehiclesApi = vehiclesApi;
            _toursApi = toursApi;
            _partnersApi = partnersApi;
            _companyApi = companyApi;
            _bookingsApi = bookingsApi;
        }

        public async Task<IActionResult> Index()
        {
            var culture = CurrentCulture;
            var model = new HomeViewModel();
            try { model.Page = await _sitePagesApi.GetFullBySlugAsync("home", culture); } catch (Exception ex) { await _appLogger.LogErrorAsync("CMS", "LoadHome", "Error loading CMS page", ex); }
            try { model.FeaturedServices = await _servicesApi.GetFeaturedAsync(culture); } catch (Exception ex) { await _appLogger.LogErrorAsync("Services", "LoadFeatured", "Error loading services", ex); }
            try { model.FeaturedVehicles = await _vehiclesApi.GetFeaturedAsync(culture); } catch (Exception ex) { await _appLogger.LogErrorAsync("Vehicles", "LoadFeatured", "Error loading vehicles", ex); }
            try { model.FeaturedTours = await _toursApi.GetFeaturedAsync(culture); } catch (Exception ex) { await _appLogger.LogErrorAsync("Tours", "LoadFeatured", "Error loading tours", ex); }
            try { model.Partners = await _partnersApi.GetActiveAsync(); } catch (Exception ex) { await _appLogger.LogErrorAsync("Partners", "LoadActive", "Error loading partners", ex); }
            try { model.Company = await _companyApi.GetCompanyProfileAsync(); } catch (Exception ex) { await _appLogger.LogErrorAsync("Company", "LoadProfile", "Error loading company profile", ex); }

            return View(model);
        }

        public async Task<IActionResult> Fleet()
        {
            var culture = CurrentCulture;
            var model = new FleetViewModel();

            try
            {
                model.Vehicles = await _vehiclesApi.GetActiveAsync(culture);
            }
            catch (Exception ex)
            {
                await _appLogger.LogErrorAsync("Vehicles", "LoadFleet", "Error loading vehicles", ex);
            }

            // Recupera i servizi attivi da usare nella modale di prenotazione
            try
            {
                model.Services = await _servicesApi.GetActiveAsync(culture);
            }
            catch (Exception ex)
            {
                await _appLogger.LogErrorAsync("Services", "LoadServicesForFleet", "Error loading services", ex);
            }

            return View(model);
        }

        public async Task<IActionResult> Tours()

        {

            var culture = CurrentCulture;

            var model = new ToursViewModel();

            try { model.Tours = await _toursApi.GetActiveAsync(culture); } catch (Exception ex) { await _appLogger.LogErrorAsync("Tours", "LoadTours", "Error loading tours", ex); }

            return View(model);

        }


        public async Task<IActionResult> TourDetail(string slug)

        {

            var culture = CurrentCulture;

            Tour? tour = null;

            // 1. Chiamata originale: Recupera i dati base del tour
            try
            {

                tour = await _toursApi.GetDetailBySlugAsync(slug, culture);

            }

            catch (Exception ex)

            {

                await _appLogger.LogErrorAsync("Tours", "LoadTourDetail", $"Error loading tour {slug}", ex);

            }

            if (tour == null) return NotFound();

            // 2. NUOVA CHIAMATA: Recupera la galleria usando l'ID del tour
            try
            {

                var gallery = await _toursApi.GetTourGalleryAsync(tour.Id);

                if (gallery != null && gallery.Any())

                {

                    // Crea la lista se è nulltour.TourGalleryImages ??= new List<TourGalleryImage>();


                    // Incolla le immagini ricevute nell'oggetto Tour
                    foreach (var img in gallery)

                    {

                        tour.TourGalleryImages.Add(new TourGalleryImage
                        {

                            ImageUrl = img.ImageUrl,

                            SortOrder = img.SortOrder

                        });

                    }

                }

            }

            catch (Exception ex)

            {

                await _appLogger.LogErrorAsync("Tours", "LoadTourGallery", $"Error loading gallery for tour {tour.Id}", ex);

            }


            var model = new TourDetailViewModel
            {

                Tour = tour
            };

            try
            {
                ViewData["PassengerOptions"] = await _bookingsApi.GetPassengerOptionsAsync(culture);
            }
            catch (Exception ex)
            {
                await _appLogger.LogErrorAsync("Bookings", "LoadPassengerOptions", "Error loading passenger options", ex);
            }

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
