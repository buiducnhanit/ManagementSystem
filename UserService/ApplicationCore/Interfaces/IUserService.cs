using ApplicationCore.DTOs;

namespace ApplicationCore.Interfaces
{
    public interface IUserService
    {
        Task<UserProfile> CreateUserAsync(CreateUserRequest request);
        Task<UserProfile> GetUserByIdAsync(Guid id);
        Task<UserProfile> UpdateUserAsync(UpdateUserRequest request);
        Task<bool> DeleteUserAsync(Guid id);
        Task<IEnumerable<UserProfile>> GetAllUsersAsync();
    }
}
