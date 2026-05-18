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
    public class ServicesController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;

        public ServicesController(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            return Ok(await _serviceRepository.GetAllAsync());
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Service>>> GetActiveServices()
        {
            return Ok(await _serviceRepository.GetActiveAsync());
        }

        [AllowAnonymous]
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<Service>>> GetFeaturedServices()
        {
            return Ok(await _serviceRepository.GetFeaturedAsync());
        }

        [AllowAnonymous]
        [HttpGet("bookable")]
        public async Task<ActionResult<IEnumerable<Service>>> GetBookableServices()
        {
            return Ok(await _serviceRepository.GetBookableAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(Guid id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null) return NotFound();
            return Ok(service);
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
