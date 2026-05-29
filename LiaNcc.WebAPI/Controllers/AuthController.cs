using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Requests;
using LiaNcc.Models.DTOs.Responses;
using LiaNcc.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IApplicationLoggerService _logger;

        public AuthController(IAuthService authService, IApplicationLoggerService logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);

                if (response == null)
                {
                    await _logger.LogInformationAsync("Auth", "Login", $"Failed login attempt for user: {request.Email}", "Auth", "User", null, "LoginFailed");
                    return Unauthorized(new { Message = "Credenziali non valide." });
                }

                await _logger.LogInformationAsync("Auth", "Login", $"User logged in: {request.Email}", "Auth", "User", null, "LoginSuccess");
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                await _logger.LogErrorAsync("Auth", "Login", $"Exception during login for user: {request.Email}", ex);
                return StatusCode(500, "Internal server error during login");
            }
        }
    }
}
