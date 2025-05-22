using ApplicationCore.DTOs;
using ApplicationCore.Interfaces;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Common.Response;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICustomLogger<AuthController> _logger;

        public AuthController(IAuthService authService, ICustomLogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var token = await _authService.RegisterAsync(dto);
                if (token == null)
                {
                    return BadRequest("Registration failed.");
                }

                _logger.Info("User {Email} registered successfully.", dto.Email);
                return Ok(ApiResponse<string>.SuccessResponse(token, "Register successful"));
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error in Register for {Email}", ex, dto.Email);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto);
                if (token == null)
                {
                    return Unauthorized("Invalid credentials.");
                }

                _logger.Info("User {Email} logged in successfully.", dto.Email);
                return Ok(ApiResponse<string>.SuccessResponse(token, "Login successful"));
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error in Login for {Email}", ex, dto.Email);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
