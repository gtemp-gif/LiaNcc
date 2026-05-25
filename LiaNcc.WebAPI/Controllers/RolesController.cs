using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Requests;
using LiaNcc.Models.DTOs.Responses;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        private readonly LiaNcc.WebAPI.Services.IApplicationLoggerService _logger;

        public RolesController(IRoleRepository roleRepository, LiaNcc.WebAPI.Services.IApplicationLoggerService logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResponse>>> GetRoles()
        {
            var roles = await _roleRepository.GetAllAsync();
            var response = roles.Select(r => new RoleResponse
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleResponse>> GetRole(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) return NotFound();

            return Ok(new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<RoleResponse>> CreateRole(CreateRoleRequest request)
        {
            var existingRole = await _roleRepository.GetByNameAsync(request.Name);
            if (existingRole != null)
                return BadRequest("Ruolo già esistente.");

            var role = new Role
            {
                Name = request.Name,
                Description = request.Description
            };

            await _roleRepository.CreateAsync(role);
            await _logger.LogInfoAsync("Auth", "CreateRole", $"Role {role.Name} created", role.Id, "Role");

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                CreatedAt = role.CreatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(Guid id, UpdateRoleRequest request)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) return NotFound();

            if (role.Name != request.Name)
            {
                var existingRole = await _roleRepository.GetByNameAsync(request.Name);
                if (existingRole != null) return BadRequest("Nome ruolo già in uso.");
            }

            role.Name = request.Name;
            role.Description = request.Description;
            role.IsActive = request.IsActive;

            await _roleRepository.UpdateAsync(role);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) return NotFound();

            // Implementing logical deletion for safety
            role.IsActive = false;
            await _roleRepository.UpdateAsync(role);
            await _logger.LogInfoAsync("Auth", "DeleteRole", $"Role {id} logically deleted", id, "Role");

            return NoContent();
        }
    }
}
