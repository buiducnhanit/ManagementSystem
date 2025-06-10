using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.JWT;
using ManagementSystem.Shared.Common.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace ApplicationCore.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICustomLogger<RefreshTokenService> _logger;
        private readonly IConfiguration _configuration;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IJwtTokenGenerator jwtTokenGenerator, UserManager<ApplicationUser> userManager, ICustomLogger<RefreshTokenService> logger,
            IConfiguration configuration)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync(ApplicationUser user, string? clientIp, bool rememberMe = false)
        {
            var durationDays = rememberMe ? Convert.ToInt32(_configuration["Jwt:RememberMeRefreshTokenDurationInDays"]!) : Convert.ToDouble(_configuration["Jwt:RefreshTokenDurationInDays"]!);
            var refreshTokenString = _jwtTokenGenerator.GenerateFreshTokenString();
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(durationDays);

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = user.Id,
                ExpiryTime = refreshTokenExpiryTime,
                CreatedAt = DateTime.UtcNow,
                LastUsed = DateTime.UtcNow,
                //IsRevoked = false,
                RevokedByIp = clientIp,
            };

            await _refreshTokenRepository.AddAsync(refreshToken);
            return refreshToken;
        }

        public async Task RevokeAllTokensForUserAsync(string userId, string reason, string? clientIp)
        {
            var existingTokens = await _refreshTokenRepository.GetAllActiveForUserAsync(userId);

            foreach (var token in existingTokens)
            {
                //token.IsRevoked = true;
                token.RevokedDate = DateTime.UtcNow;
                token.ReasonRevoked = reason;
                token.RevokedByIp = clientIp;
                await _refreshTokenRepository.UpdateAsync(token);
            }
        }

        public async Task RevokeSpecificTokenAsync(string tokenString, string userId, string reason, string? clientIp)
        {
            var token = await _refreshTokenRepository.GetAsync(tokenString, userId);
            if (token == null || !token.IsActive)
            {
                _logger.Warn("Attempted to revoke a non-existing or inactive refresh token for user {userId}", userId);
                return;
            }
            //token.IsRevoked = true;
            token.RevokedDate = DateTime.UtcNow;
            token.ReasonRevoked = reason;
            token.RevokedByIp = clientIp;
            await _refreshTokenRepository.UpdateAsync(token);
            _logger.Debug("Revoked refresh token for user {userId} with reason: {reason}", userId, reason);
        }

        public async Task<(string newAccessToken, RefreshToken newRefreshToken)?> RotateRefreshTokenAsync(string oldRefreshToken, string userId, string? clientIp)
        {
            var existingToken = await _refreshTokenRepository.GetAsync(oldRefreshToken, userId);

            if (existingToken == null || !existingToken.IsActive)
            {
                _logger.Warn("Attempted to rotate a non-existing or inactive refresh token for user {userId}", null, null, userId);
                return null;
            }

            var timeSinceLastUsed = DateTime.UtcNow - existingToken.LastUsed;
            var _idleSessionTimeoutHours = Convert.ToInt32(_configuration["Session:IdleSessionTimeoutInHours"]!);
            var idleSessionTimeoutMinutes = _idleSessionTimeoutHours * 60;
            if (timeSinceLastUsed.TotalMinutes >= idleSessionTimeoutMinutes)
            {
                _logger.Warn("Refresh token for user {userId} has been inactive for too long. Last used: {LastUsed}, now: {DateTime}", null, null, userId, existingToken.LastUsed.ToString(), DateTime.UtcNow);
                await RevokeAllTokensForUserAsync(userId, $"Session idle timeout exceeded ({_idleSessionTimeoutHours} hours)", clientIp);
                return null;
            }

            if (existingToken.IsRevoked || existingToken.ExpiryTime < DateTime.UtcNow)
            {
                _logger.Warn("Attempted to rotate a revoked or expired refresh token for user {userId}.", null, null, userId);
                await RevokeAllTokensForUserAsync(userId, "Attempt to use revoked/expired token for rotation", clientIp);
                return null;
            }

            //existingToken.IsRevoked = true;
            existingToken.RevokedDate = DateTime.UtcNow;
            existingToken.ReasonRevoked = "Rotated for new token";
            existingToken.RevokedByIp = clientIp;
            await _refreshTokenRepository.UpdateAsync(existingToken);
            _logger.Debug("Revoked old refresh token for user {userId} with reason: {ReasonRevoked}", null, null, userId, existingToken.ReasonRevoked);

            var user = existingToken.User ?? await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.Warn("User not found for ID {UserId} while rotating refresh token", null, null, userId);
                return null;
            }

            var newAccessToken = await _jwtTokenGenerator.GenerateTokenAsync(user);

            var newRefreshToken = await GenerateRefreshTokenAsync(user, clientIp);

            _logger.Debug("Generated new refresh token for user {UserId} with expiry {ExpiryTime}", null, null, user.Id, newRefreshToken.ExpiryTime.ToString());

            return (newAccessToken, newRefreshToken);
        }
    }
}
