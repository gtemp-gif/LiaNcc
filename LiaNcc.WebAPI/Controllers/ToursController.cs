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
    [Authorize(Roles = "Admin,Operator")]
    public class ToursController : ControllerBase
    {
        private readonly ITourRepository _tourRepository;

        public ToursController(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tour>>> GetTours()
        {
            return Ok(await _tourRepository.GetAllAsync());
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Tour>>> GetActiveTours()
        {
            return Ok(await _tourRepository.GetActiveToursAsync());
        }

        [AllowAnonymous]
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<Tour>>> GetFeaturedTours()
        {
            return Ok(await _tourRepository.GetFeaturedToursAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Tour>> GetTour(Guid id)
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null) return NotFound();
            return Ok(tour);
        }

        [AllowAnonymous]
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<Tour>> GetTourBySlug(string slug)
        {
            var tour = await _tourRepository.GetBySlugAsync(slug);
            if (tour == null) return NotFound();
            return Ok(tour);
        }

        [AllowAnonymous]
        [HttpGet("{id}/detail")]
        public async Task<ActionResult<Tour>> GetTourDetail(Guid id)
        {
            var tour = await _tourRepository.GetTourDetailAsync(id);
            if (tour == null) return NotFound();
            return Ok(tour);
        }

        [AllowAnonymous]
        [HttpGet("slug/{slug}/detail")]
        public async Task<ActionResult<Tour>> GetTourDetailBySlug(string slug)
        {
            var tour = await _tourRepository.GetTourDetailBySlugAsync(slug);
            if (tour == null) return NotFound();
            return Ok(tour);
        }

        [AllowAnonymous]
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Tour>>> GetByCategory(Guid categoryId)
        {
            return Ok(await _tourRepository.GetByCategoryAsync(categoryId));
        }

        [AllowAnonymous]
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<TourCategory>>> GetCategories()
        {
            return Ok(await _tourRepository.GetCategoriesAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Tour>> CreateTour(Tour tour)
        {
            await _tourRepository.CreateAsync(tour);
            return Ok(tour);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTour(Guid id, Tour tour)
        {
            if (id != tour.Id) return BadRequest();
            await _tourRepository.UpdateAsync(tour);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTour(Guid id)
        {
            await _tourRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
