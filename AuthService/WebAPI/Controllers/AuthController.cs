using ApplicationCore.DTOs;
using ApplicationCore.Interfaces;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Common.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ICustomLogger<AuthController> _logger;

        public AuthController(IAuthService authService, IRefreshTokenService refreshTokenService, ICustomLogger<AuthController> logger)
        {
            _authService = authService;
            _refreshTokenService = refreshTokenService;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
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

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    _logger.Warn($"Invalid refresh token request for user {dto.UserId}. Errors: {string.Join(", ", errors)}");
                    return BadRequest(ApiResponse<string>.FailureResponse("Invalid refresh token data.", 400, errors));
                }

                var refreshedToken = await _refreshTokenService.RotateRefreshTokenAsync(dto.RefreshToken, dto.UserId, HttpContext.Connection.RemoteIpAddress?.ToString());

                if (refreshedToken == null)
                {
                    _logger.Warn($"Failed to refresh token for user {dto.UserId}. Invalid or expired refresh token.");
                    return Unauthorized(ApiResponse<string>.FailureResponse("Invalid or expired refresh token.", 401));
                }

                var (newAccessToken, newRefreshToken) = refreshedToken.Value;
                _logger.Info($"User {dto.UserId} refreshed token successfully.");

                return Ok(ApiResponse<RefreshTokenResponseDto>.SuccessResponse(new RefreshTokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken.Token,
                    ExpiryTime = newRefreshToken.ExpiryTime
                }, "Token refreshed successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error($"Unexpected error in RefreshToken for user {dto.UserId}", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to refresh token.", 400, ex.Errors));
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.Warn("Logout attempt with no user ID found in claims.");
                    return BadRequest(ApiResponse<string>.FailureResponse("User not authenticated.", 400));
                }

                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    _logger.Warn($"Logout attempt for non-existing user ID {userId}.");
                    return NotFound(ApiResponse<string>.FailureResponse("User not found.", 404));
                }

                await _refreshTokenService.RevokeAllTokensForUserAsync(user.Id.ToString(), "User logged out", HttpContext.Connection.RemoteIpAddress?.ToString());
                _logger.Info($"User {user.Email} logged out successfully. All tokens revoked.");
                await _authService.UpdateSecurityStampAsync(user.Id.ToString());

                return Ok(ApiResponse<string>.SuccessResponse(null, "Logout successful."));
            }
            catch (HandleException ex)
            {
                _logger.Error($"Unexpected error in Logout for user ID {User.FindFirstValue(ClaimTypes.NameIdentifier)}", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Logout failed.", 400, ex.Errors));
            }
        }
    }
}
