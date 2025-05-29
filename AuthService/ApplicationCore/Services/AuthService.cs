using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.JWT;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

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

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenGenerator jwtTokenGenerator, ICustomLogger<AuthService> logger,
            IConfiguration configuration, ISendMailService sendMailService, IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
            _configuration = configuration;
            _sendMailService = sendMailService;
            _refreshTokenService = refreshTokenService;
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
                    Email = dto.Email
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    _logger.Warn("Failed to create user. Reasons: {@Reasons}", errors);

                    throw new HandleException("User registration failed.", 400, errors);
                }

                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = $"{_configuration["Frontend:BaseUrl"]}/verify-email?userId={user.Id}&token={Uri.EscapeDataString(emailToken)}";

                var subject = "Confirm your email";
                var body = $"<p>Hello {user.UserName},</p>" +
                           $"<p>Click <a href='{confirmationLink}'>here</a> to confirm your email address.</p>";

                //await _sendMailService.SendEmailAsync(user.Email!, subject, body, isHtml: true);

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

                var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user, clientIp);
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
    }
}
