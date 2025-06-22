using AuthService.Data;
using AuthService.Entities;
using AuthService.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IdentityDbContext _context;

        public RefreshTokenRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RefreshToken>> GetAllActiveForUserAsync(string userId)
        {
            return await _context.RefreshTokens
                .Where(rt => rt.UserId.ToString() == userId && rt.RevokedDate == null && rt.ExpiryTime >= DateTime.UtcNow)
                .Include(rt => rt.User)
                .ToListAsync();
        }

        public async Task<RefreshToken?> GetAsync(string token, string userId)
        {
            return await _context.RefreshTokens
                .SingleOrDefaultAsync(rt => rt.Token == token && rt.UserId.ToString() == userId && rt.RevokedDate == null && rt.ExpiryTime > DateTime.UtcNow);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeTokensInactiveFor(TimeSpan inactivity)
        {
            var cutoffTime = DateTime.UtcNow.Subtract(inactivity);

            var tokensToRevoke = await _context.RefreshTokens
                .Where(r => !r.RevokedDate.HasValue &&
                            r.ExpiryTime > DateTime.UtcNow &&
                            r.LastUsed < cutoffTime)
                .ToListAsync();

            foreach (var token in tokensToRevoke)
            {
                token.RevokedDate = DateTime.UtcNow;
                token.ReasonRevoked = "Inactive session timeout";
            }
            await _context.SaveChangesAsync();
        }
    }
}
