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

        private readonly IApplicationLoggerService _logger;

        public UsersController(IUsersApiClient usersApiClient, IApplicationLoggerService applicationLogger)
        {
            _usersApiClient = usersApiClient;
            _logger = applicationLogger;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _usersApiClient.GetAllAsync();
            return View(users);
        }
    }
}
