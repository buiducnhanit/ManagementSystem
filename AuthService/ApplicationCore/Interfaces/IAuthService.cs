using ApplicationCore.DTOs;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto?> LoginAsync(LoginDto dto, string clientIp);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task UpdateSecurityStampAsync(string id);
    }
}
