using AuthService.Entities;

namespace AuthService.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetAsync(string token, string userId);
        Task UpdateAsync(RefreshToken token);
        Task<List<RefreshToken>> GetAllActiveForUserAsync(string userId);
        Task SaveChangesAsync();
    }
}
