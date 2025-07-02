using WebAPI.DTOs;

namespace WebAPI.Interfaces
{
    public interface IUserService
    {
        Task<UserProfile> CreateUserAsync(CreateUserRequest request);
        Task<UserProfile?> GetUserByIdAsync(Guid id);
        Task<UserProfile> UpdateUserAsync(Guid id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(Guid id);
        Task<IEnumerable<UserProfile>> GetAllUsersAsync();
        Task<bool> UnLockUserAsync(Guid id);
    }
}
