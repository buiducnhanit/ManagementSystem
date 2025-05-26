using ApplicationCore.DTOs;
using ApplicationCore.Interfaces;
using ManagementSystem.Shared.Common.Exceptions;
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

                _logger.Info($"User {dto.Email} registered successfully.");
                return Ok(ApiResponse<string>.SuccessResponse(token, "Register successful"));
            }
            catch (HandleException ex)
            {
                _logger.Error($"Unexpected error in Register for {dto.Email}", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("User registration failed.", 400, ex.Errors));
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

                _logger.Info($"User {dto.Email} logged in successfully.");
                return Ok(ApiResponse<string>.SuccessResponse(token, "Login successful"));
            }
            catch (HandleException ex)
            {
                _logger.Error($"Unexpected error in Login for {dto.Email}", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("User logged failed.", 400, ex.Errors));
            }
        }
    }
}
