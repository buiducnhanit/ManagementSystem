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

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenGenerator jwtTokenGenerator, ICustomLogger<AuthService> logger,
            IConfiguration configuration, ISendMailService sendMailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
            _configuration = configuration;
            _sendMailService = sendMailService;
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

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                    throw new HandleException("Email does not exit.", 404);

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

                var domainUser = new ApplicationUser { Id = user.Id, UserName = user.UserName!, Email = user.Email! };

                return _jwtTokenGenerator.GenerateToken(domainUser);
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
    }
}
