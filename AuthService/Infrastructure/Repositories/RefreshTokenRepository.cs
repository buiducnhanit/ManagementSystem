﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Services
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
                .Where(rt => rt.UserId.ToString() == userId && !rt.IsRevoked && rt.ExpiryTime >= DateTime.UtcNow)
                .Include(rt => rt.User)
                .ToListAsync();
        }

        public async Task<RefreshToken?> GetAsync(string token, string userId)
        {
            return await _context.RefreshTokens
                .SingleOrDefaultAsync(rt => rt.Token == token && rt.UserId.ToString() == userId && rt.IsActive);
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
    }
}
