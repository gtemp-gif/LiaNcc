using System;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Dashboard;
using LiaNcc.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
            try
            {
                var summary = await _dashboardRepository.GetSummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                // In a real app, log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
