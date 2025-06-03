using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GenerateRefreshTokenAsync(ApplicationUser user, string? clientIp, bool rememberMe = false);
        Task<(string newAccessToken, RefreshToken newRefreshToken)?> RotateRefreshTokenAsync(string oldRefreshToken, string userId, string? clientIp);
        Task RevokeAllTokensForUserAsync(string userId, string reason, string? clientIp);
        Task RevokeSpecificTokenAsync(string tokenString, string userId, string reason, string? clientIp);
    }
}
