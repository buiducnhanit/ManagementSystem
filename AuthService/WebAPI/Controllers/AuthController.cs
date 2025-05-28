using ApplicationCore.DTOs;
using ApplicationCore.Interfaces;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Common.Response;
using Microsoft.AspNetCore.Authorization;
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
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    _logger.Warn($"Invalid registration attempt for {dto.Email}. Errors: {string.Join(", ", errors)}");
                    return BadRequest(ApiResponse<string>.FailureResponse("Invalid registration data.", 400, errors));
                }
                await _authService.RegisterAsync(dto);

                _logger.Info($"User {dto.Email} registered successfully. Confirmation email sent.");
                return Ok(ApiResponse<string>.SuccessResponse(null, "Registration successful. Please check your email to confirm your account."));
            }
            catch (HandleException ex)
            {
                _logger.Error($"Unexpected error in Register for {dto.Email}", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("User registration failed.", 400, ex.Errors));
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                var loginResponse = await _authService.LoginAsync(dto, clientIp);
                if (loginResponse == null)
                {
                    return Unauthorized(ApiResponse<string>.FailureResponse("Invalid credentials.", 401));
                }

                _logger.Info($"User {dto.Email} logged in successfully.");
                return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(loginResponse, "Login successful"));
            }
            catch (HandleException ex)
            {
                _logger.Error($"Unexpected error in Login for {dto.Email}", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("User logged failed.", 400, ex.Errors));
            }
        }
    }
}
