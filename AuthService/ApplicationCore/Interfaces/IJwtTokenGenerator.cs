using ApplicationCore.Entities;

namespace Infrastructure.JWT
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser user);
    }
}
