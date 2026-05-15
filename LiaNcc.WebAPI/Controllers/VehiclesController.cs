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
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehiclesController(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
        {
            var vehicles = await _vehicleRepository.GetActiveAsync();
            return Ok(vehicles);
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetAllVehicles()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return Ok(vehicles);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Vehicle>> GetVehicle(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) return NotFound();
            return Ok(vehicle);
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpPost]
        public async Task<ActionResult<Vehicle>> CreateVehicle(Vehicle vehicle)
        {
            await _vehicleRepository.CreateAsync(vehicle);
            return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehicle);
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(Guid id, Vehicle vehicle)
        {
            if (id != vehicle.Id) return BadRequest();

            var existing = await _vehicleRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _vehicleRepository.UpdateAsync(vehicle);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            var existing = await _vehicleRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _vehicleRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
