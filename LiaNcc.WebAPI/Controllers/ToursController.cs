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
        private readonly IMediaRepository _mediaRepository;
        private readonly ILocalizedContentRepository _localizationRepository;
        private readonly LiaNcc.WebAPI.Helpers.ILocalizationResolver _resolver;
        private readonly LiaNcc.WebAPI.Services.IApplicationLoggerService _logger;

        public ToursController(
            ITourRepository tourRepository,
            IMediaRepository mediaRepository,
            ILocalizedContentRepository localizationRepository,
            LiaNcc.WebAPI.Helpers.ILocalizationResolver resolver,
            LiaNcc.WebAPI.Services.IApplicationLoggerService logger)
        {
            _tourRepository = tourRepository;
            _mediaRepository = mediaRepository;
            _localizationRepository = localizationRepository;
            _resolver = resolver;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TourDto>>> GetTours([FromQuery] string? culture)
        {
            var tours = await _tourRepository.GetAllAsync();
            var dtos = new List<TourDto>();
            foreach (var t in tours)
            {
                if (!string.IsNullOrEmpty(culture)) await LocalizeTour(t, culture);
                dtos.Add(await MapToDto(t));
            }
            return Ok(dtos);
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<TourDto>>> GetActiveTours([FromQuery] string? culture)
        {
            var tours = await _tourRepository.GetActiveToursAsync();
            var dtos = new List<TourDto>();
            foreach (var t in tours)
            {
                if (!string.IsNullOrEmpty(culture)) await LocalizeTour(t, culture);
                dtos.Add(await MapToDto(t));
            }
            return Ok(dtos);
        }

        [AllowAnonymous]
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<TourDto>>> GetFeaturedTours([FromQuery] string? culture)
        {
            var tours = await _tourRepository.GetFeaturedToursAsync();
            var dtos = new List<TourDto>();
            foreach (var t in tours)
            {
                if (!string.IsNullOrEmpty(culture)) await LocalizeTour(t, culture);
                dtos.Add(await MapToDto(t));
            }
            return Ok(dtos);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<TourDto>> GetTour(Guid id, [FromQuery] string? culture)
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null) return NotFound();
            if (!string.IsNullOrEmpty(culture)) await LocalizeTour(tour, culture);
            return Ok(await MapToDto(tour));
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
            await _logger.LogInformationAsync("Tours", "CreateTour", $"Tour {tour.Name} created", "Tours", "Tour", tour.Id);
            return Ok(tour);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTour(Guid id, Tour tour)
        {
            if (id != tour.Id) return BadRequest();
            await _tourRepository.UpdateAsync(tour);
            await _logger.LogInformationAsync("Tours", "UpdateTour", $"Tour {tour.Name} updated", "Tours", "Tour", tour.Id);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{id}/gallery")]
        public async Task<ActionResult<IEnumerable<TourGalleryImageDto>>> GetTourGallery(Guid id)
        {
            var media = await _mediaRepository.GetMediaForEntityAsync("Tours", id);
            var gallery = media.Where(m => m.MediaType == "Gallery")
                .Select(m => new TourGalleryImageDto
                {
                    Id = m.Id,
                    ImageUrl = m.MediaAsset.FileUrl,
                    SortOrder = m.SortOrder
                }).OrderBy(m => m.SortOrder);
            return Ok(gallery);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTour(Guid id)
        {
            await _tourRepository.DeleteAsync(id);
            await _logger.LogInformationAsync("Tours", "DeleteTour", $"Tour {id} deleted", "Tours", "Tour", id);
            return NoContent();
        }

        [HttpDelete("gallery/{imageId}")]
        public async Task<IActionResult> DeleteGalleryImage(Guid imageId)
        {
            await _mediaRepository.RemoveMediaFromEntityAsync(imageId);
            await _logger.LogInformationAsync("Tours", "DeleteGalleryImage", $"Gallery image {imageId} removed", "Tours", "TourGalleryImage", imageId);
            return NoContent();
        }

        private async Task<TourDto> MapToDto(Tour t)
        {
            var media = await _mediaRepository.GetMediaForEntityAsync("Tours", t.Id);

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
                IsBookable = t.IsBookable,
                IsActive = t.IsActive,
                SortOrder = t.SortOrder,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                GalleryImages = media.Where(m => m.MediaType == "Gallery").Select(m => new TourGalleryImageDto
                {
                    Id = m.Id,
                    ImageUrl = m.MediaAsset.FileUrl,
                    SortOrder = m.SortOrder
                }).OrderBy(m => m.SortOrder).ToList()
            };
        }
    }
}
