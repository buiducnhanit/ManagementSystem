using IdentityService.Application.DTOs;

namespace IdentityService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResultDto> LoginAsync(LoginRequestDto request);
    }
}
