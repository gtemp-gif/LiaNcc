using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using System.Threading.Tasks;
using System.Linq;

namespace LiaNcc.BO.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IToursApiClient _toursApiClient;
        private readonly IVehiclesApiClient _vehiclesApiClient;
        // We will add more clients as needed

        public HomeController(IToursApiClient toursApiClient, IVehiclesApiClient vehiclesApiClient)
        {
            _toursApiClient = toursApiClient;
            _vehiclesApiClient = vehiclesApiClient;
        }

        public async Task<IActionResult> Index()
        {
            // Simple statistics fetching
            var tours = await _toursApiClient.GetAllAsync();
            var vehicles = await _vehiclesApiClient.GetAllVehiclesAsync();

            ViewBag.ActiveToursCount = tours.Count(t => t.IsActive);
            ViewBag.ActiveVehiclesCount = vehicles.Count(v => v.IsActive);

            // To be expanded with real counts for Bookings, Messages, etc.
            ViewBag.PendingBookingsCount = 0;
            ViewBag.UnreadMessagesCount = 0;

            return View();
        }
    }
}
