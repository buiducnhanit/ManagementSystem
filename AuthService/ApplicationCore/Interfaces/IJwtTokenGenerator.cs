using ApplicationCore.Entities;

namespace Infrastructure.JWT
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateTokenAsync(ApplicationUser user);
    }
}
