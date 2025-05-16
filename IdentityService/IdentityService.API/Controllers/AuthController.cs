using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Common.Response;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ICustomLogger<AuthController> _logger;
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService, ICustomLogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            _logger.Info("Received register request", dto);

            var result = await _authService.RegisterAsync(dto);

            if (result.Success)
            {
                _logger.Info("Register successful", new { dto.Email });
                return Ok(ApiResponse<string>.SuccessResponse(result.Token!, "Register successful"));
            }
            else
            {
                _logger.Warn("Register failed", result.Errors);
                return BadRequest(ApiResponse<string>.FailureResponse(string.Join("; ", result.Errors!)));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            _logger.Info("Received login request", new { dto.UserName });

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var result = await _authService.LoginAsync(dto);

            if (result.Success)
            {
                _logger.Info("User logged in", new { IP = ipAddress, dto.UserName });
                return Ok(ApiResponse<string>.SuccessResponse(result.Token!, "Login successful"));
            }
            else
            {
                _logger.Warn("Login failed", result.Errors);
                return BadRequest(ApiResponse<string>.FailureResponse(string.Join("; ", result.Errors!)));
            }
        }
    }
}
