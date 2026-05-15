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
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UsersController(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            var response = users.Select(u => new UserResponse
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                IsActive = u.IsActive,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            });
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> CreateUser(CreateUserRequest request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest("Email già in uso.");

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _userRepository.CreateAsync(user);

            foreach (var roleId in request.RoleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role != null)
                {
                    await _userRepository.AssignRoleAsync(user.Id, roleId);
                }
            }

            var createdUser = await _userRepository.GetByIdAsync(user.Id); // Reload with roles

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserResponse
            {
                Id = createdUser!.Id,
                FullName = createdUser.FullName,
                Email = createdUser.Email,
                IsActive = createdUser.IsActive,
                Roles = createdUser.UserRoles.Select(ur => ur.Role.Name).ToList(),
                CreatedAt = createdUser.CreatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.IsActive = request.IsActive;

            await _userRepository.UpdateAsync(user);

            // Update roles (simplified: remove all, add new)
            var currentRoles = await _userRepository.GetUserRolesAsync(id);
            foreach (var role in currentRoles)
            {
                await _userRepository.RemoveRoleAsync(id, role.Id);
            }

            foreach (var roleId in request.RoleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role != null)
                {
                    await _userRepository.AssignRoleAsync(id, roleId);
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            await _userRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{userId}/roles/{roleId}")]
        public async Task<IActionResult> AssignRole(Guid userId, Guid roleId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null) return NotFound("Role not found.");

            await _userRepository.AssignRoleAsync(userId, roleId);
            return NoContent();
        }

        [HttpDelete("{userId}/roles/{roleId}")]
        public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId)
        {
            await _userRepository.RemoveRoleAsync(userId, roleId);
            return NoContent();
        }
    }
}
