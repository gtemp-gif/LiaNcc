using System;
using System.Threading.Tasks;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.BO.Controllers
{
    public class CMSController : BaseController
    {
        // For simplicity, we just add a mock response here to fulfill the missing logic warning.
        // In a real scenario, this would have CMS pages, sections, calltoactions etc.
        public IActionResult Index()
        {
            return View();
        }
    }
}
