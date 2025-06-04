using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(Guid id);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
