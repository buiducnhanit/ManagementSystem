using Asp.Versioning;
using AuthService.DTOs;
using AuthService.Interfaces;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Common.Response;
using ManagementSystem.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.Controllers
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
                await _authService.RegisterAsync(dto);

                _logger.Info("User {Email} registered successfully. Confirmation email sent.", null, null, dto.Email);
                return Ok(ApiResponse<string?>.SuccessResponse(null, "Registration successful. Please check your email to confirm your account."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error in Register for {Email}", ex, null, null, dto.Email);
                return StatusCode(ex.StatusCode, ApiResponse<string>.FailureResponse(ex.Message, ex.StatusCode, ex.Errors));
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
                _logger.Info("User {Email} logged in successfully from IP: {clientIp} at UTC: {loginTime}.", propertyValues: [dto.Email, clientIp!, loginTime]);

                return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(loginResponse!, "Login successful"));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error in Login for {Email}", ex, null, null, dto.Email);
                return StatusCode(ex.StatusCode, ApiResponse<string>.FailureResponse(ex.Message, ex.StatusCode, ex.Errors));
            }
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            try
            {
                var ipClient = HttpContext.Connection.RemoteIpAddress?.ToString();
                var refreshTokenResponse = await _authService.RefreshTokenAsync(dto, ipClient);

                return Ok(ApiResponse<RefreshTokenResponseDto>.SuccessResponse(refreshTokenResponse, "Token refreshed successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error in RefreshToken for user {UserId}", ex, null, null, dto.UserId);
                return StatusCode(ex.StatusCode, ApiResponse<string>.FailureResponse(ex.Message, ex.StatusCode, ex.Errors));
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
                    _logger.Warn("Logout attempt with no user {ID} found in claims.", null, null, User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                    return BadRequest(ApiResponse<string>.FailureResponse("User not authenticated.", 400));
                }

                await _authService.LogoutAsync(userId, HttpContext.Connection.RemoteIpAddress?.ToString());
                await _authService.UpdateSecurityStampAsync(userId);

                return Ok(ApiResponse<string>.SuccessResponse("Logout successful."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error in Logout for user ID {ID}", ex, null, null, User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                return BadRequest(ApiResponse<string>.FailureResponse("Logout failed.", 400, ex.Errors));
            }
        }

        //[Authorize]
        //[HttpGet("get-profile")]
        //public async Task<IActionResult> GetProfile()
        //{
        //    try
        //    {
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            _logger.Warn("Profile request with no user ID found in claims.");
        //            return BadRequest(ApiResponse<string>.FailureResponse("User not authenticated.", 400));
        //        }
        //        var user = await _authService.GetUserByIdAsync(userId);
        //        if (user == null)
        //        {
        //            _logger.Warn($"Profile request for non-existing user ID {userId}.");
        //            return NotFound(ApiResponse<string>.FailureResponse("User not found.", 404));
        //        }
        //        var profileDto = new UserProfileDto
        //        {
        //            UserName = user.UserName,
        //            Email = user.Email,
        //            Roles = await _authService.GetUserRolesAsync(user.Id.ToString())
        //        };
        //        _logger.Info($"User profile retrieved successfully for {user.Email}.");
        //        return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profileDto, "Profile retrieved successfully."));
        //    }
        //    catch (HandleException ex)
        //    {
        //        _logger.Error($"Unexpected error in GetProfile for user ID {User.FindFirstValue(ClaimTypes.NameIdentifier)}", ex);
        //        return BadRequest(ApiResponse<string>.FailureResponse("Failed to retrieve profile.", 400, ex.Errors));
        //    }
        //}

        //[Authorize]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest dto)
        {
            try
            {
                await _authService.ForgotPasswordAsyns(dto);
                _logger.Info("Generate reset password token successfully for User: {Email}", null, null, dto.Email);

                return Ok(ApiResponse<string?>.SuccessResponse(null, "Generate reset password token successfully. Confirm your email to set new password."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error when generate reset password token.", ex);
                return StatusCode(ex.StatusCode, ApiResponse<string>.FailureResponse(ex.Message, ex.StatusCode, ex.Errors));
            }
        }

        //[Authorize]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest dto)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(dto.UserId);
                var result = await _authService.ResetPasswordAsyns(dto);
                _logger.Info("Reset password successfully for User: {Email}", null, null, user.Email!);

                return Ok(ApiResponse<string?>.SuccessResponse(null, "Reset password successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error when reset password.", ex);
                return StatusCode(ex.StatusCode, ApiResponse<string>.FailureResponse(ex.Message, ex.StatusCode, ex.Errors));
            }
        }

        [HttpPost("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Token))
                {
                    _logger.Debug("Email confirmation attempted with missing user ID or token.");
                    return BadRequest(ApiResponse<string>.FailureResponse("Invalid email confirmation request.", 400));
                }
                var result = await _authService.ConfirmEmailAsync(request);
                if (!result)
                {
                    _logger.Debug("Email confirmation failed for user ID {userId} with token {token}.", null, null, request.UserId, request.Token);
                    return BadRequest(ApiResponse<string>.FailureResponse("Email confirmation failed.", 400));
                }
                _logger.Info("Email confirmed successfully for user ID {userId}.", null, null, request.UserId);
                return Ok(ApiResponse<string>.SuccessResponse("Email confirmed successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error in ConfirmEmail for user ID {userId}", ex, null, null, request.UserId);
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to confirm email.", 400, ex.Errors));
            }
        }

        [HttpPost("resend-confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    _logger.Warn("Resend email confirmation attempted with missing email.");
                    return BadRequest(ApiResponse<string>.FailureResponse("Invalid email confirmation request.", 400));
                }
                var result = await _authService.ResendEmailConfirmationAsync(request);
                if (!result)
                {
                    _logger.Warn("Resend email confirmation failed for email {email}.", null, null, request.Email);
                    return BadRequest(ApiResponse<string>.FailureResponse("Resend email confirmation failed.", 400));
                }
                _logger.Info("Email confirmation resent successfully for email {email}.", null, null, request.Email);
                return Ok(ApiResponse<string?>.SuccessResponse(null, "Email confirmation resent successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error in ResendEmailConfirmation for email {email}", ex, null, null, request.Email);
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to resend email confirmation.", 400, ex.Errors));
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.OldPassword) || string.IsNullOrEmpty(request.NewPassword))
                {
                    _logger.Warn("Change password attempted with missing user ID or passwords.");
                    return BadRequest(ApiResponse<string>.FailureResponse("Invalid change password request.", 400));
                }

                if (request.NewPassword != request.ConfirmPassword)
                {
                    _logger.Warn("Change password failed for user ID {userId} due to password mismatch.");
                    return BadRequest(ApiResponse<string>.FailureResponse("New password and confirmation do not match.", 400));
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _authService.ChangePasswordAsync(userId!, request);
                if (!result)
                {
                    _logger.Warn("Change password failed for user ID {userId}.");
                    return BadRequest(ApiResponse<string>.FailureResponse("Change password failed.", 400));
                }
                _logger.Info("Password changed successfully for user ID {userId}.");
                return Ok(ApiResponse<string>.SuccessResponse(null!, "Password changed successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error in ChangePassword for user ID {userId}", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to change password.", 400, ex.Errors));
            }
        }

        [HttpPut("user-info")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateAuthEvent command)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(command.Id.ToString());
                if (user == null)
                {
                    _logger.Info("User with ID {UserId} not found in AuthService for internal update.", null, null, command.Id);
                    return NotFound($"User with ID: {command.Id} not found in AuthService.");
                }

                await _authService.UpdateUserInfoAsync(command);
                _logger.Info("User info updated successfully for user ID: {UserId}", null, null, command.Id);
                return Ok(ApiResponse<string?>.SuccessResponse(null, "User info updated successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error in UpdateUserInfo for user ID {UserId}", ex, null, null, command.Id);
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to update user info.", 400, ex.Errors));
            }
        }

        [HttpPost("create-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUserByAdmin([FromBody] CreateUserByAdminRequest request)
        {
            try
            {
                _logger.Info("Admin ({AdminEmail}) attempting to create new user with Email: {Email}", null, null, User.Identity?.Name!, request.Email);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newUser = await _authService.CreateUserByAdminAsync(request);
                return Ok(ApiResponse<string>.SuccessResponse("User created successfully user for email: {Email}. Password sent to user email.", newUser.Email!));
            }
            catch (HandleException ex)
            {
                _logger.Error("Unexpected error in CreateUserByAdmin for email {Email}", ex, null, null, request.Email);
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to create user.", 400, ex.Errors));
            }
        }

        [HttpPost("unlockout")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnLockOut([FromBody] UnLockOutRequest request)
        {
            try
            {
                var response = await _authService.UnLockOutAsync(request);
                if (response)
                {
                    return Ok(ApiResponse<string?>.SuccessResponse(null, "Unlock out for user successfully.", StatusCodes.Status200OK));
                }

                return BadRequest(ApiResponse<string>.FailureResponse("Failed to unlock out user.", StatusCodes.Status400BadRequest));
            }
            catch (HandleException ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse("Unexpected error occurred while unlock out user.", StatusCodes.Status400BadRequest, ex.Errors));
            }
        }
    }
}
