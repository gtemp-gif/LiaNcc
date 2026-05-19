using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;

namespace LiaNcc.BO.Controllers
{
    public class BookingsController : BaseController
    {
        private readonly IBookingsApiClient _bookingsApiClient;

        public BookingsController(IBookingsApiClient bookingsApiClient)
        {
            _bookingsApiClient = bookingsApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingsApiClient.GetAllAsync();
            return View(bookings);
        }
    }
}
