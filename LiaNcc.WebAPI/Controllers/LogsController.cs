using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Logging;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using LiaNcc.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/logs")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly IApplicationLogRepository _repository;
        private readonly IApplicationLoggerService _loggerService;

        public LogsController(IApplicationLogRepository repository, IApplicationLoggerService loggerService)
        {
            _repository = repository;
            _loggerService = loggerService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedLogsResponse>> GetLogs([FromQuery] ApplicationLogFilterRequest filter)
        {
            return Ok(await _repository.GetPagedAsync(filter));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationLogDto>> GetLog(long id)
        {
            var log = await _repository.GetByIdAsync(id);
            if (log == null) return NotFound();
            return Ok(log);
        }

        [AllowAnonymous] // Allow public logging from FE/BO
        [HttpPost]
        public async Task<IActionResult> CreateLog(CreateApplicationLogRequest request)
        {
            await _loggerService.LogAsync(request);
            return Ok();
        }

        [HttpDelete("cleanup")]
        [Authorize] // Allow cleanup
        public async Task<IActionResult> Cleanup([FromQuery] int olderThanDays = 30)
        {
            await _repository.DeleteOlderThanAsync(DateTime.UtcNow.AddDays(-olderThanDays));
            return NoContent();
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var today = DateTime.UtcNow.Date;
            var last7Days = DateTime.UtcNow.AddDays(-7);

            var stats = new
            {
                ErrorsToday = await _repository.CountErrorsAsync(today),
                WarningsToday = await _repository.CountByLevelAsync("Warning", today),
                ErrorsLast7Days = await _repository.CountErrorsAsync(last7Days),
                CriticalsTotal = await _repository.CountByLevelAsync("Critical")
            };

            return Ok(stats);
        }
    }
}
