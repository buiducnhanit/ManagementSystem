using AuthService.Entities;

namespace AuthService.Interfaces
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateTokenAsync(ApplicationUser user);
        string GenerateFreshTokenString();
        string GenerateInternalServiceToken();
    }
}
