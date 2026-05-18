using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;

namespace LiaNcc.BO.Controllers
{
    public class BookingsController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
