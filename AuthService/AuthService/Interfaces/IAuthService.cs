using AuthService.DTOs;
using AuthService.Entities;
using ManagementSystem.Shared.Contracts;
using Microsoft.AspNetCore.Authentication;

namespace AuthService.Interfaces
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
        Task ForgotPasswordAsyns(ForgotPasswordRequest request);
        Task<bool> ResetPasswordAsyns(ResetPasswordRequest request);
        Task<bool> ConfirmEmailAsync(ConfirmEmailRequest request);
        Task<bool> ResendEmailConfirmationAsync(ResendEmailConfirmationRequest request);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task UpdateUserInfoAsync(UpdateAuthEvent request);
        Task<ApplicationUser> CreateUserByAdminAsync(CreateUserByAdminRequest request);
        Task<bool> UnLockOutAsync(UnLockOutRequest request);
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string v, string redirectUrl);
        Task<LoginResponseDto> HandleExternalLoginAsync(string redirectUrl, string clientIp);
    }
}

