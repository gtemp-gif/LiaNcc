using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Tours;
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
        private readonly ILocalizedContentRepository _localizationRepository;
        private readonly LiaNcc.WebAPI.Helpers.ILocalizationResolver _resolver;

        public ToursController(
            ITourRepository tourRepository,
            ILocalizedContentRepository localizationRepository,
            LiaNcc.WebAPI.Helpers.ILocalizationResolver resolver)
        {
            _tourRepository = tourRepository;
            _localizationRepository = localizationRepository;
            _resolver = resolver;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TourDto>>> GetTours([FromQuery] string? culture)
        {
            var tours = await _tourRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(culture))
            {
                foreach (var tour in tours) await LocalizeTour(tour, culture);
            }
            return Ok(tours.Select(MapToDto));
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<TourDto>>> GetActiveTours([FromQuery] string? culture)
        {
            var tours = await _tourRepository.GetActiveToursAsync();
            if (!string.IsNullOrEmpty(culture))
            {
                foreach (var tour in tours) await LocalizeTour(tour, culture);
            }
            return Ok(tours.Select(MapToDto));
        }

        [AllowAnonymous]
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<TourDto>>> GetFeaturedTours([FromQuery] string? culture)
        {
            var tours = await _tourRepository.GetFeaturedToursAsync();
            if (!string.IsNullOrEmpty(culture))
            {
                foreach (var tour in tours) await LocalizeTour(tour, culture);
            }
            return Ok(tours.Select(MapToDto));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<TourDto>> GetTour(Guid id, [FromQuery] string? culture)
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null) return NotFound();
            if (!string.IsNullOrEmpty(culture)) await LocalizeTour(tour, culture);
            return Ok(MapToDto(tour));
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
        public async Task<ActionResult<Tour>> GetTourDetail(Guid id, [FromQuery] string? culture)
        {
            var tour = await _tourRepository.GetTourDetailAsync(id);
            if (tour == null) return NotFound();
            if (!string.IsNullOrEmpty(culture)) await LocalizeTour(tour, culture);
            return Ok(tour);
        }

        private async Task LocalizeTour(Tour tour, string culture)
        {
            var translations = await _localizationRepository.GetByEntityAsync("Tour", tour.Id, culture);
            tour.Name = _resolver.Resolve(translations, "Name", tour.Name, culture);
            tour.Description = _resolver.Resolve(translations, "Description", tour.Description ?? "", culture);
            tour.HeroTitle = _resolver.Resolve(translations, "HeroTitle", tour.HeroTitle ?? "", culture);
            tour.HeroSubtitle = _resolver.Resolve(translations, "HeroSubtitle", tour.HeroSubtitle ?? "", culture);
            tour.ExperienceDescription = _resolver.Resolve(translations, "ExperienceDescription", tour.ExperienceDescription ?? "", culture);
            tour.MeetingPoint = _resolver.Resolve(translations, "MeetingPoint", tour.MeetingPoint ?? "", culture);
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

        private static TourDto MapToDto(Tour t)
        {
            return new TourDto
            {
                Id = t.Id,
                CategoryId = t.CategoryId,
                CategoryName = t.TourCategory?.Name,
                Name = t.Name,
                Slug = t.Slug,
                Price = t.Price,
                CoverImageUrl = t.CoverImageUrl,
                Description = t.Description,
                HeroTitle = t.HeroTitle,
                HeroSubtitle = t.HeroSubtitle,
                ExperienceDescription = t.ExperienceDescription,
                ExperienceImageUrl = t.ExperienceImageUrl,
                DurationDays = t.DurationDays,
                DurationHours = t.DurationHours,
                MeetingPoint = t.MeetingPoint,
                VehicleId = t.VehicleId,
                VehicleName = t.Vehicle != null ? $"{t.Vehicle.Name} ({t.Vehicle.Title})" : null,
                IsFeatured = t.IsFeatured,
                IsActive = t.IsActive,
                SortOrder = t.SortOrder,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            };
        }
    }
}
