using System;
using System.Threading.Tasks;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.DTOs.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.BO.Controllers
{
    [Authorize]
    public class LogController : Controller
    {
        private readonly ILogsApiClient _logsApiClient;

        public LogController(ILogsApiClient logsApiClient)
        {
            _logsApiClient = logsApiClient;
        }

        public async Task<IActionResult> Index(ApplicationLogFilterRequest filter)
        {
            filter.PageSize = 50;
            var result = await _logsApiClient.GetLogsAsync(filter);
            ViewBag.Filter = filter;
            return View(result);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var log = await _logsApiClient.GetLogByIdAsync(id);
            if (log == null) return NotFound();
            return View(log);
        }

        [HttpPost]
        public async Task<IActionResult> Cleanup(int olderThanDays = 30)
        {
            await _logsApiClient.CleanupAsync(olderThanDays);
            TempData["SuccessMessage"] = "Log ripuliti correttamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
