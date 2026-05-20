using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Vehicles;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly ILocalizedContentRepository _localizationRepository;
        private readonly LiaNcc.WebAPI.Helpers.ILocalizationResolver _resolver;

        public VehiclesController(
            IVehicleRepository vehicleRepository,
            IMediaRepository mediaRepository,
            ILocalizedContentRepository localizationRepository,
            LiaNcc.WebAPI.Helpers.ILocalizationResolver resolver)
        {
            _vehicleRepository = vehicleRepository;
            _mediaRepository = mediaRepository;
            _localizationRepository = localizationRepository;
            _resolver = resolver;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetVehicles([FromQuery] string? culture)
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            var dtos = new List<VehicleDto>();
            foreach (var v in vehicles)
            {
                var fullV = await _vehicleRepository.GetVehicleWithFeaturesAsync(v.Id);
                if (fullV != null)
                {
                    if (!string.IsNullOrEmpty(culture)) await LocalizeVehicle(fullV, culture);
                    dtos.Add(await MapToDto(fullV));
                }
            }
            return Ok(dtos);
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetActiveVehicles([FromQuery] string? culture)
        {
            var vehicles = await _vehicleRepository.GetActiveVehiclesAsync();
            var dtos = new List<VehicleDto>();
            foreach (var v in vehicles)
            {
                var fullV = await _vehicleRepository.GetVehicleWithFeaturesAsync(v.Id);
                if (fullV != null)
                {
                    if (!string.IsNullOrEmpty(culture)) await LocalizeVehicle(fullV, culture);
                    dtos.Add(await MapToDto(fullV));
                }
            }
            return Ok(dtos);
        }

        [AllowAnonymous]
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetFeaturedVehicles([FromQuery] string? culture)
        {
            var vehicles = await _vehicleRepository.GetFeaturedVehiclesAsync();
            var dtos = new List<VehicleDto>();
            foreach (var v in vehicles)
            {
                var fullV = await _vehicleRepository.GetVehicleWithFeaturesAsync(v.Id);
                if (fullV != null)
                {
                    if (!string.IsNullOrEmpty(culture)) await LocalizeVehicle(fullV, culture);
                    dtos.Add(await MapToDto(fullV));
                }
            }
            return Ok(dtos);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleDto>> GetVehicle(Guid id, [FromQuery] string? culture)
        {
            var vehicle = await _vehicleRepository.GetVehicleWithFeaturesAsync(id);
            if (vehicle == null) return NotFound();
            if (!string.IsNullOrEmpty(culture)) await LocalizeVehicle(vehicle, culture);
            return Ok(await MapToDto(vehicle));
        }

        private async Task LocalizeVehicle(Vehicle vehicle, string culture)
        {
            var translations = await _localizationRepository.GetByEntityAsync("Vehicle", vehicle.Id, culture);
            vehicle.Name = _resolver.Resolve(translations, "Name", vehicle.Name, culture);
            vehicle.Title = _resolver.Resolve(translations, "Title", vehicle.Title ?? "", culture);
            vehicle.Description = _resolver.Resolve(translations, "Description", vehicle.Description ?? "", culture);

            foreach (var feature in vehicle.VehicleFeatures)
            {
                var fTranslations = await _localizationRepository.GetByEntityAsync("VehicleFeature", feature.Id, culture);
                feature.Name = _resolver.Resolve(fTranslations, "Name", feature.Name, culture);
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}/gallery")]
        public async Task<ActionResult<IEnumerable<VehicleGalleryImageDto>>> GetVehicleGallery(Guid id)
        {
            var media = await _mediaRepository.GetMediaForEntityAsync("Vehicles", id);
            var gallery = media.Where(m => m.MediaType == "Gallery")
                .Select(m => new VehicleGalleryImageDto
                {
                    Id = m.Id,
                    ImageUrl = m.MediaAsset.FileUrl,
                    SortOrder = m.SortOrder
                }).OrderBy(m => m.SortOrder);
            return Ok(gallery);
        }

        [AllowAnonymous]
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetByCategory(Guid categoryId)
        {
            return Ok(await _vehicleRepository.GetByCategoryAsync(categoryId));
        }

        [AllowAnonymous]
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<VehicleCategory>>> GetCategories()
        {
            return Ok(await _vehicleRepository.GetCategoriesAsync());
        }

        [AllowAnonymous]
        [HttpGet("categories/active")]
        public async Task<ActionResult<IEnumerable<VehicleCategory>>> GetActiveCategories()
        {
            return Ok(await _vehicleRepository.GetActiveCategoriesAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Vehicle>> CreateVehicle(VehicleUpsertRequest request)
        {
            var vehicle = new Vehicle
            {
                CategoryId = request.CategoryId,
                Name = request.Name,
                Title = request.Title,
                Description = request.Description,
                Seats = request.Seats,
                Luggages = request.Luggages,
                IsFeatured = request.IsFeatured,
                IsBookable = request.IsBookable,
                IsActive = request.IsActive,
                SortOrder = request.SortOrder
            };

            await _vehicleRepository.CreateAsync(vehicle);

            foreach (var f in request.Features)
            {
                await _vehicleRepository.AddFeatureAsync(new VehicleFeature
                {
                    VehicleId = vehicle.Id,
                    Name = f.Name,
                    Icon = f.Icon,
                    SortOrder = f.SortOrder
                });
            }

            return Ok(vehicle);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(Guid id, VehicleUpsertRequest request)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) return NotFound();

            vehicle.CategoryId = request.CategoryId;
            vehicle.Name = request.Name;
            vehicle.Title = request.Title;
            vehicle.Description = request.Description;
            vehicle.Seats = request.Seats;
            vehicle.Luggages = request.Luggages;
            vehicle.IsFeatured = request.IsFeatured;
            vehicle.IsBookable = request.IsBookable;
            vehicle.IsActive = request.IsActive;
            vehicle.SortOrder = request.SortOrder;

            await _vehicleRepository.UpdateAsync(vehicle);

            await _vehicleRepository.ClearFeaturesAsync(id);
            foreach (var f in request.Features)
            {
                await _vehicleRepository.AddFeatureAsync(new VehicleFeature
                {
                    VehicleId = id,
                    Name = f.Name,
                    Icon = f.Icon,
                    SortOrder = f.SortOrder
                });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            await _vehicleRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete("gallery/{imageId}")]
        public async Task<IActionResult> DeleteGalleryImage(Guid imageId)
        {
            await _mediaRepository.RemoveMediaFromEntityAsync(imageId);
            return NoContent();
        }

        private async Task<VehicleDto> MapToDto(Vehicle v)
        {
            var media = await _mediaRepository.GetMediaForEntityAsync("Vehicles", v.Id);

            return new VehicleDto
            {
                Id = v.Id,
                CategoryId = v.CategoryId,
                CategoryName = v.VehicleCategory?.Name,
                Name = v.Name,
                Title = v.Title,
                Description = v.Description,
                Seats = v.Seats,
                Luggages = v.Luggages,
                IsFeatured = v.IsFeatured,
                IsBookable = v.IsBookable,
                IsActive = v.IsActive,
                SortOrder = v.SortOrder,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt,
                Features = v.VehicleFeatures.Select(f => new VehicleFeatureDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Icon = f.Icon,
                    SortOrder = f.SortOrder
                }).ToList(),
                GalleryImages = media.Where(m => m.MediaType == "Gallery").Select(m => new VehicleGalleryImageDto
                {
                    Id = m.Id,
                    ImageUrl = m.MediaAsset.FileUrl,
                    SortOrder = m.SortOrder
                }).OrderBy(m => m.SortOrder).ToList()
            };
        }
    }
}
