using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Infrastructure.Repositories
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterRequestDto request)
        {
            var user = new ApplicationUser { UserName = request.UserName, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description).ToList() };
            }

            var domainUser = new User { Id = user.Id, UserName = user.UserName!, Email = user.Email! };
            var token = _tokenGenerator.GenerateToken(domainUser);
            return new AuthResultDto { Success = true, Token = token };
        }

        public async Task<AuthResultDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return new AuthResultDto { Success = false, Errors = ["Invalid credentials"] };

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return new AuthResultDto { Success = false, Errors = ["Invalid credentials"] };

            var domainUser = new User { Id = user.Id, UserName = user.UserName!, Email = user.Email! };
            var token = _tokenGenerator.GenerateToken(domainUser);
            return new AuthResultDto { Success = true, Token = token };
        }
    }
}
