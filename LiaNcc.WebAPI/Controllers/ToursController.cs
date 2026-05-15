using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ToursController : ControllerBase
    {
        private readonly ITourRepository _tourRepository;

        public ToursController(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tour>>> GetTours()
        {
            var tours = await _tourRepository.GetActiveAsync();
            return Ok(tours);
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Tour>>> GetAllTours()
        {
            var tours = await _tourRepository.GetAllAsync();
            return Ok(tours);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Tour>> GetTour(Guid id)
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null) return NotFound();
            return Ok(tour);
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpPost]
        public async Task<ActionResult<Tour>> CreateTour(Tour tour)
        {
            await _tourRepository.CreateAsync(tour);
            return CreatedAtAction(nameof(GetTour), new { id = tour.Id }, tour);
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTour(Guid id, Tour tour)
        {
            if (id != tour.Id) return BadRequest();

            var existing = await _tourRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _tourRepository.UpdateAsync(tour);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTour(Guid id)
        {
            var existing = await _tourRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _tourRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
