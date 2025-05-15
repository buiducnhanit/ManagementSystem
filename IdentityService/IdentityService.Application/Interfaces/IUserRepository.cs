using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(Guid userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<User> GetUserByUsernameAsync(string username);
        Task<bool> UserExistsAsync(string email);
        Task<bool> UserExistsAsync(Guid userId);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> SaveChangesAsync();
    }
}
