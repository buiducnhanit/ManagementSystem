using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetAsync(string token, string userId);
        Task UpdateAsync(RefreshToken token);
        Task<List<RefreshToken>> GetAllActiveForUserAsync(Guid userId);
        Task SaveChangesAsync();
    }
}
