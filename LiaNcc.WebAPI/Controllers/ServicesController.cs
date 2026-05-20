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
    public class ServicesController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILocalizedContentRepository _localizationRepository;
        private readonly LiaNcc.WebAPI.Helpers.ILocalizationResolver _resolver;

        public ServicesController(
            IServiceRepository serviceRepository,
            ILocalizedContentRepository localizationRepository,
            LiaNcc.WebAPI.Helpers.ILocalizationResolver resolver)
        {
            _serviceRepository = serviceRepository;
            _localizationRepository = localizationRepository;
            _resolver = resolver;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices([FromQuery] string? culture)
        {
            var services = await _serviceRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(culture))
            {
                foreach (var service in services)
                {
                    await LocalizeService(service, culture);
                }
            }
            return Ok(services);
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Service>>> GetActiveServices([FromQuery] string? culture)
        {
            var services = await _serviceRepository.GetActiveAsync();
            if (!string.IsNullOrEmpty(culture))
            {
                foreach (var service in services)
                {
                    await LocalizeService(service, culture);
                }
            }
            return Ok(services);
        }

        [AllowAnonymous]
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<Service>>> GetFeaturedServices([FromQuery] string? culture)
        {
            var services = await _serviceRepository.GetFeaturedAsync();
            if (!string.IsNullOrEmpty(culture))
            {
                foreach (var service in services)
                {
                    await LocalizeService(service, culture);
                }
            }
            return Ok(services);
        }

        [AllowAnonymous]
        [HttpGet("bookable")]
        public async Task<ActionResult<IEnumerable<Service>>> GetBookableServices([FromQuery] string? culture)
        {
            var services = await _serviceRepository.GetBookableAsync();
            if (!string.IsNullOrEmpty(culture))
            {
                foreach (var service in services)
                {
                    await LocalizeService(service, culture);
                }
            }
            return Ok(services);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(Guid id, [FromQuery] string? culture)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null) return NotFound();
            if (!string.IsNullOrEmpty(culture))
            {
                await LocalizeService(service, culture);
            }
            return Ok(service);
        }

        private async Task LocalizeService(Service service, string culture)
        {
            var translations = await _localizationRepository.GetByEntityAsync("Service", service.Id, culture);
            service.Name = _resolver.Resolve(translations, "Name", service.Name, culture);
            service.Description = _resolver.Resolve(translations, "Description", service.Description ?? "", culture);
        }

        [HttpPost]
        public async Task<ActionResult<Service>> CreateService(Service service)
        {
            await _serviceRepository.CreateAsync(service);
            return Ok(service);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(Guid id, Service service)
        {
            if (id != service.Id) return BadRequest();
            await _serviceRepository.UpdateAsync(service);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            await _serviceRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
