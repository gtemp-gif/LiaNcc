using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;

namespace LiaNcc.BO.Controllers
{
    public class PartnersController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
