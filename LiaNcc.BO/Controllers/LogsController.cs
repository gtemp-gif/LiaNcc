using System;
using System.Threading.Tasks;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.DTOs.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.BO.Controllers
{
    [Authorize]
    public class LogsController : Controller
    {
        private readonly ILogsApiClient _logsApiClient;

        public LogsController(ILogsApiClient logsApiClient)
        {
            _logsApiClient = logsApiClient;
        }

        public async Task<IActionResult> Index(ApplicationLogFilterRequest filter)
        {
            if (filter.Page <= 0) filter.Page = 1;
            if (filter.PageSize <= 0) filter.PageSize = 50;
            
            var result = await _logsApiClient.GetLogsAsync(filter);
            ViewBag.Filter = filter;
            return View(result);
        }

        public async Task<IActionResult> Details(long id)
        {
            var log = await _logsApiClient.GetLogByIdAsync(id);
            if (log == null) return NotFound();
            return View(log);
        }

        [HttpPost]
        public async Task<IActionResult> CleanupLogs(int olderThanDays = 30)
        {
            if (olderThanDays <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Il numero di giorni non è valido."
                });
            }

            try
            {
                await _logsApiClient.CleanupAsync(olderThanDays);

                return Ok(new
                {
                    success = true,
                    message = "Log ripuliti correttamente."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Errore durante la pulizia dei log.",
                    exception = ex.Message
                });
            }
        }
    }
}
