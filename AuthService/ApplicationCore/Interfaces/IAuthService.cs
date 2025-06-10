using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ManagementSystem.Shared.Contracts;

namespace ApplicationCore.Interfaces
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto?> LoginAsync(LoginDto dto, string clientIp);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task UpdateSecurityStampAsync(string id);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto, string? clientIp);
        Task LogoutAsync(string userId, string? clientIp);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task ForgotPasswordAsyns(ForgotPasswordRequest request);
        Task<bool> ResetPasswordAsyns(ResetPasswordRequest request);
        Task<bool> ConfirmEmailAsync(ConfirmEmailRequest request);
        Task<bool> ResendEmailConfirmationAsync(ResendEmailConfirmationRequest request);
        Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
        Task UpdateUserInfoAsync(UpdateAuthEvent request);
        Task<ApplicationUser> CreateUserByAdminAsync(CreateUserByAdminRequest request);
    }
}

