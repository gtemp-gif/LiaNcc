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
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehiclesController(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
        {
            return Ok(await _vehicleRepository.GetAllAsync());
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetActiveVehicles()
        {
            return Ok(await _vehicleRepository.GetActiveVehiclesAsync());
        }

        [AllowAnonymous]
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetFeaturedVehicles()
        {
            return Ok(await _vehicleRepository.GetFeaturedVehiclesAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Vehicle>> GetVehicle(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) return NotFound();
            return Ok(vehicle);
        }

        [AllowAnonymous]
        [HttpGet("{id}/features")]
        public async Task<ActionResult<Vehicle>> GetVehicleWithFeatures(Guid id)
        {
            var vehicle = await _vehicleRepository.GetVehicleWithFeaturesAsync(id);
            if (vehicle == null) return NotFound();
            return Ok(vehicle);
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

        [HttpPost]
        public async Task<ActionResult<Vehicle>> CreateVehicle(Vehicle vehicle)
        {
            await _vehicleRepository.CreateAsync(vehicle);
            return Ok(vehicle);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(Guid id, Vehicle vehicle)
        {
            if (id != vehicle.Id) return BadRequest();
            await _vehicleRepository.UpdateAsync(vehicle);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            await _vehicleRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
