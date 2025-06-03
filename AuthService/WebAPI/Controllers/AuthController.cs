using ApplicationCore.DTOs;
using ApplicationCore.Interfaces;
using Asp.Versioning;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Common.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
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
                return Ok(ApiResponse<string>.SuccessResponse("Registration successful. Please check your email to confirm your account."));
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
                var loginTime = DateTime.UtcNow;
                var loginResponse = await _authService.LoginAsync(dto, clientIp!);
                if (loginResponse == null)
                {
                    return Unauthorized(ApiResponse<string>.FailureResponse("Invalid credentials.", 401));
                }

                _logger.Info($"User {dto.Email} logged in successfully from IP: {clientIp} at UTC: {loginTime}.");
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

                var ipClient = HttpContext.Connection.RemoteIpAddress?.ToString();
                var refreshTokenResponse = await _authService.RefreshTokenAsync(dto, ipClient);

                return Ok(ApiResponse<RefreshTokenResponseDto>.SuccessResponse(refreshTokenResponse, "Token refreshed successfully."));
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

                await _authService.LogoutAsync(userId, HttpContext.Connection.RemoteIpAddress?.ToString());
                await _authService.UpdateSecurityStampAsync(userId);

                return Ok(ApiResponse<string>.SuccessResponse("Logout successful."));
            }
            catch (HandleException ex)
            {
                _logger.Error($"Unexpected error in Logout for user ID {User.FindFirstValue(ClaimTypes.NameIdentifier)}", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Logout failed.", 400, ex.Errors));
            }
        }

        [Authorize]
        [HttpGet("get-profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.Warn("Profile request with no user ID found in claims.");
                    return BadRequest(ApiResponse<string>.FailureResponse("User not authenticated.", 400));
                }
                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    _logger.Warn($"Profile request for non-existing user ID {userId}.");
                    return NotFound(ApiResponse<string>.FailureResponse("User not found.", 404));
                }
                var profileDto = new UserProfileDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = await _authService.GetUserRolesAsync(user.Id.ToString())
                };
                _logger.Info($"User profile retrieved successfully for {user.Email}.");
                return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profileDto, "Profile retrieved successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error($"Unexpected error in GetProfile for user ID {User.FindFirstValue(ClaimTypes.NameIdentifier)}", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to retrieve profile.", 400, ex.Errors));
            }
        }

        [Authorize]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest dto)
        {
            try
            {
                await _authService.ForgotPasswordAsyns(dto);
                _logger.Info("Generate reset password token successfully for User: {Email}", dto.Email);

                return Ok(ApiResponse<string>.SuccessResponse("Generate reset password token successfully. Confirm your email to set new password."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error when generate reset password token.", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to generate reset password token.", 400, ex.Errors));
            }
        }

        [Authorize]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest dto)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(dto.UserId);
                var result = await _authService.ResetPasswordAsyns(dto);
                _logger.Info("Reset password successfully for User: {Email}", user.Email);

                return Ok(ApiResponse<string>.SuccessResponse("Reset password successfully. Confirm your email to set new password."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error when reset password.", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to reset password.", 400, ex.Errors));
            }
        }

        [HttpPost("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Token))
                {
                    _logger.Warn("Email confirmation attempted with missing user ID or token.");
                    return BadRequest(ApiResponse<string>.FailureResponse("Invalid email confirmation request.", 400));
                }
                var result = await _authService.ConfirmEmailAsync(request);
                if (!result)
                {
                    _logger.Warn("Email confirmation failed for user ID {userId} with token {token}.", request.UserId, request.Token);
                    return BadRequest(ApiResponse<string>.FailureResponse("Email confirmation failed.", 400));
                }
                _logger.Info("Email confirmed successfully for user ID {userId}.", request.UserId);
                return Ok(ApiResponse<string>.SuccessResponse("Email confirmed successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error in ConfirmEmail for user ID {userId}", ex, request.UserId);
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to confirm email.", 400, ex.Errors));
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.OldPassword) || string.IsNullOrEmpty(request.NewPassword))
                {
                    _logger.Warn("Change password attempted with missing user ID or passwords.");
                    return BadRequest(ApiResponse<string>.FailureResponse("Invalid change password request.", 400));
                }

                if (request.NewPassword != request.ConfirmPassword)
                {
                    _logger.Warn("Change password failed for user ID {userId} due to password mismatch.", request.UserId);
                    return BadRequest(ApiResponse<string>.FailureResponse("New password and confirmation do not match.", 400));
                }

                var result = await _authService.ChangePasswordAsync(request);
                if (!result)
                {
                    _logger.Warn("Change password failed for user ID {userId}.", request.UserId);
                    return BadRequest(ApiResponse<string>.FailureResponse("Change password failed.", 400));
                }
                _logger.Info("Password changed successfully for user ID {userId}.", request.UserId);
                return Ok(ApiResponse<string>.SuccessResponse("Password changed successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error in ChangePassword for user ID {userId}", ex, request.UserId);
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to change password.", 400, ex.Errors));
            }
        }
    }
}
