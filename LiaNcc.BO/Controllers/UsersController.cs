using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;

namespace LiaNcc.BO.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUsersApiClient _usersApiClient;

        public UsersController(IUsersApiClient usersApiClient)
        {
            _usersApiClient = usersApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _usersApiClient.GetAllAsync();
            return View(users);
        }
    }
}
