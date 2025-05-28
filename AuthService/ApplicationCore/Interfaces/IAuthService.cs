using ApplicationCore.DTOs;

namespace ApplicationCore.Interfaces
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto?> LoginAsync(LoginDto dto, string? clientIp);
    }
}
