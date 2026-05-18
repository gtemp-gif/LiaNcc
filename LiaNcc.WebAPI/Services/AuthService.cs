using System;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Requests;
using LiaNcc.Models.DTOs.Responses;
using LiaNcc.Repository.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LiaNcc.WebAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailWithRolesAsync(request.Email);

            if (user == null || !user.IsActive)
            {
                return null; // Invalid credentials or inactive user
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                return null;
            }

            var roles = user.UserRoles.Select(ur => ur.Role).ToList();
            var token = _jwtTokenService.GenerateToken(user, roles);

            return new LoginResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Roles = roles.Select(r => r.Name).ToList(),
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "240"))
            };
        }
    }
}
