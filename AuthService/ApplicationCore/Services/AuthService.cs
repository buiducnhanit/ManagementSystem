using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.JWT;
using ManagementSystem.Shared.Common.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ApplicationCore.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenGenerator jwtTokenGenerator, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
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
                    throw new HandleException("User registration failed.", 400);
                }

                var domainUser = new ApplicationUser
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!
                };

                var token = _jwtTokenGenerator.GenerateToken(domainUser);
                return token;
            }
            catch (HandleException)
            {
                throw; // HandleException will be caught in the middleware and logged there
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in RegisterAsync for {Email}", dto.Email);
                throw new HandleException("An unexpected error occurred during registration.", 500);
            }
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                    throw new HandleException("Invalid credentials", 400);

                var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
                if (!result.Succeeded)
                    throw new HandleException("Invalid credentials", 400);

                var domainUser = new ApplicationUser { Id = user.Id, UserName = user.UserName!, Email = user.Email! };
                var token = _jwtTokenGenerator.GenerateToken(domainUser);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in LoginAsync for {Email}", dto.Email);
                throw new HandleException("An unexpected error occurred during login.", 500);
            }
        }
    }
}
