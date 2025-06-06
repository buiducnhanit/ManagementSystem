using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.JWT;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;

namespace ApplicationCore.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ICustomLogger<AuthService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISendMailService _sendMailService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenGenerator jwtTokenGenerator, ICustomLogger<AuthService> logger,
            IConfiguration configuration, ISendMailService sendMailService, IRefreshTokenService refreshTokenService, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
            _configuration = configuration;
            _sendMailService = sendMailService;
            _refreshTokenService = refreshTokenService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string?> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var existingUser = await _userManager.FindByNameAsync(dto.UserName) ?? await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    throw new HandleException("Email is already registered.", 400);
                }

                var user = new ApplicationUser
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Address = dto.Address,
                    PhoneNumber = dto.PhoneNumber,
                    DateOfBirth = dto.DateOfBirth,
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    _logger.Warn("Failed to create user. Reasons: {@Reasons}", errors);

                    throw new HandleException("User registration failed.", 400, errors);
                }

                await _userManager.AddToRoleAsync(user, "User");
                await CallUserServiceToCreateProfile(user);

                // Send confirmation email
                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = $"{_configuration["Frontend:BaseUrl"]}/verify-email?userId={user.Id}&token={Uri.EscapeDataString(emailToken)}";

                var subject = "Confirm your email";
                var body = $"<p>Hello {user.UserName},</p>" +
                           $"<p>Click <a href='{confirmationLink}'>here</a> to confirm your email address.</p>";

                await _sendMailService.SendEmailAsync(user.Email!, subject, body, isHtml: true);

                _logger.Info("User {Email} registered successfully and confirmation email sent.", user.Email);

                return null;
            }
            catch (HandleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error in RegisterAsync for {dto.Email}", ex);
                throw new HandleException("An unexpected error occurred during registration.", 500);
            }
        }

        private async Task CallUserServiceToCreateProfile(ApplicationUser user)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["UserService:BaseUrl"]!);

                var internalToken = _jwtTokenGenerator.GenerateInternalServiceToken();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", internalToken);

                var createUserRequest = new
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    AvatarUrl = user.AvatarUrl,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                };

                var response = await client.PostAsJsonAsync("api/v1/users", createUserRequest);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.Error("Failed to create user profile in user service.");
                    throw new HandleException("Failed to create user profile in user service.", (int)response.StatusCode);
                }
                _logger.Info("User profile created successfully for user {UserId} ({Email}).", user.Id, user.Email);
            }
            catch (Exception ex)
            {
                _logger.Error("Error calling user service to create profile for user {UserId} ({Email}).", ex, user.Id, user.Email);
                throw new HandleException("An unexpected error occurred while creating user profile.", 500);
            }
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto, string clientIp)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    _logger.Warn("Login failed. Reason: User not found for email {Email}", dto.Email);
                    throw new HandleException("Invalid credentials.", 401, new List<string> { "Email or password incorrect." });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
                if (!result.Succeeded)
                {
                    if (result.IsNotAllowed)
                    {
                        var errors = new List<string>();

                        if (!user.EmailConfirmed)
                            errors.Add("Email is not confirmed.");

                        if (!user.PhoneNumberConfirmed)
                            errors.Add("Phone number is not confirmed.");

                        _logger.Warn("Login not allowed for user {Email}. Reasons: {@Reasons}", dto.Email);

                        throw new HandleException("Login not allowed.", 403, errors);
                    }

                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        _logger.Warn("Login failed. Reason: User is locked out: {Email}", dto.Email);
                        throw new HandleException("Login failed.", 400, new List<string> { "Account is locked out." });
                    }

                    if (!await _userManager.HasPasswordAsync(user))
                    {
                        _logger.Warn("Login failed. Reason: User has no password set: {Email}", dto.Email);
                        throw new HandleException("Login failed.", 400, new List<string> { "No password set for this account." });
                    }

                    _logger.Warn("Login failed. Reason: Invalid password for user {Email}", dto.Email);
                    throw new HandleException("Login failed.", 400, new List<string> { "Incorrect password." });
                }

                await _userManager.UpdateSecurityStampAsync(user);
                _logger.Info("Security stamp updated for user {UserId} ({Email}).", user.Id, user.Email);

                await _refreshTokenService.RevokeAllTokensForUserAsync(user.Id.ToString(), "New login detected", clientIp);
                _logger.Info("Revoked all existing refresh tokens for user {UserId} ({Email}) due to new login.", user.Id, user.Email);

                var accessToken = await _jwtTokenGenerator.GenerateTokenAsync(user);
                var accessTokenExpires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]!));
                _logger.Info("Generated new access token for user {UserId} ({Email}).", user.Id, user.Email);

                var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user, clientIp, dto.RememberMe);
                _logger.Info("Generated new refresh token for user {UserId} ({Email}).", user.Id, user.Email);

                return new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresIn = accessTokenExpires,
                    UserId = user.Id
                };
            }
            catch (HandleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error in LoginAsync for {Email}", ex, dto.Email);
                throw new HandleException("An unexpected error occurred during login.", 500);
            }
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.Warn("User not found for ID {UserId}", userId);
                throw new HandleException("User not found.", 404);
            }
            _logger.Info("User {UserId} ({Email}) retrieved successfully.", user.Id, user.Email);
            return user;
        }

        public async Task UpdateSecurityStampAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.Warn("User not found for ID {UserId}", id);
                throw new HandleException("User not found.", 404);
            }
            await _userManager.UpdateSecurityStampAsync(user);
            _logger.Info("Security stamp updated for user {UserId} ({Email}).", user.Id, user.Email);
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.Warn("User not found for ID {UserId}", userId);
                throw new HandleException("User not found.", 404);
            }
            var roles = await _userManager.GetRolesAsync(user);
            _logger.Info("Roles retrieved for user {UserId} ({Email}): {@Roles}", user.Id, user.Email, roles);
            return roles.ToList();
        }

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto, string? clientIp)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(dto.UserId);
                if (user == null)
                {
                    _logger.Warn("User not found for ID {UserId}", dto.UserId);
                    throw new HandleException("User not found.", 404);
                }
                var refreshedToken = await _refreshTokenService.RotateRefreshTokenAsync(dto.RefreshToken, dto.UserId, clientIp);

                if (refreshedToken == null)
                {
                    _logger.Warn("Failed to refresh token for user {UserId}. Invalid or expired refresh token.", dto.UserId);
                    throw new HandleException("Invalid or expired refresh token.", 401);
                }

                var newAccessToken = refreshedToken.Value.newAccessToken;
                var newRefreshToken = refreshedToken.Value.newRefreshToken;
                _logger.Info("User {UserId} refreshed token successfully.", dto.UserId);

                return new RefreshTokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken.Token,
                    ExpiryTime = newRefreshToken.ExpiryTime
                };
            }
            catch (HandleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error in RefreshTokenAsync for {UserId}", ex, dto.UserId);
                throw new HandleException("An unexpected error occurred during token refresh.", 500);
            }
        }

        public async Task LogoutAsync(string userId, string? clientIp)
        {
            try
            {
                string reason = "User logged out";
                await _refreshTokenService.RevokeAllTokensForUserAsync(userId, reason, clientIp);
                _logger.Info("All tokens revoked for user {userId} due to logout. Reason: {reason}", userId, reason);
            }
            catch (Exception ex)
            {
                _logger.Error("Error revoking tokens for user {userId} during logout.", ex, userId);
                throw new HandleException("An unexpected error occurred during logout.", 500);
            }
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.Warn("User not found for email {Email}", email);
                    throw new HandleException("User not found.", 404);
                }

                _logger.Info("User {UserId} ({Email}) retrieved successfully.", user.Id, user.Email);
                return user;
            }
            catch (Exception ex)
            {
                _logger.Error("Fail to retrieved user.", ex);
                throw new HandleException("An unexpected error occurred during logout.", 500);
            }
        }

        public async Task ForgotPasswordAsyns(ForgotPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.Info("Email is invalid.");
                    throw new HandleException("Email is invalid.", 400);
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                _logger.Info("Generate password reset token successfull.");

                // Send email to reset password
                var resetPasswordLink = $"{_configuration["FrontendBaseUrl"]}/reset-password?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";
                await _sendMailService.SendEmailAsync(request.Email, "Reset password", $"Click the link to reset your password: <a href='{resetPasswordLink}'>Reset Password</a>");
            }
            catch (Exception ex)
            {
                _logger.Error("Error generate password reset token.", ex);
                throw new HandleException("An unexpected error occurred during generate password reset token.", 500);
            }
        }

        public async Task<bool> ResetPasswordAsyns(ResetPasswordRequest dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(dto.UserId);
                if (user == null)
                {
                    _logger.Error("User not found.");
                    throw new HandleException("User not found.", 404);
                }

                var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
                if (!result.Succeeded)
                {
                    _logger.Error("Failed to reset password.");
                    throw new HandleException("Failed to reset password.", 400, result.Errors.Select(e => e.Description).ToList());
                }

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.Error("Error reset password.", ex);
                throw new HandleException("An unexpected error occurred during reset password.", 500);
            }
        }

        public async Task<bool> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    _logger.Warn("User not found for ID {UserId}", request.UserId);
                    throw new HandleException("User not found.", 404);
                }
                var result = await _userManager.ConfirmEmailAsync(user, request.Token);
                if (!result.Succeeded)
                {
                    _logger.Warn("Failed to confirm email for user {UserId}.", request.UserId);
                    throw new HandleException("Email confirmation failed.", 400, result.Errors.Select(e => e.Description).ToList());
                }
                _logger.Info("Email confirmed successfully for user {UserId} ({Email}).", user.Id, user.Email);
                return true;
            }
            catch (HandleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error in ConfirmEmailAsync for {userId}", ex, request.UserId);
                throw new HandleException("An unexpected error occurred during email confirmation.", 500);
            }
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    _logger.Warn("User not found for ID {UserId}", request.UserId);
                    throw new HandleException("User not found.", 404);
                }
                var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    _logger.Warn("Failed to change password for user {UserId}.", request.UserId);
                    throw new HandleException("Failed to change password.", 400, result.Errors.Select(e => e.Description).ToList());
                }
                _logger.Info("Password changed successfully for user {UserId} ({Email}).", user.Id, user.Email);
                return true;
            }
            catch (HandleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error in ChangePasswordAsync for {userId}", ex, request.UserId);
                throw new HandleException("An unexpected error occurred during password change.", 500);
            }
        }
    }
}
