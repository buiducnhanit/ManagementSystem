using WebAPI.Entities;
using WebAPI.Interfaces;
using WebAPI.Data;
using ManagementSystem.Shared.Common.Interfaces;
using ManagementSystem.Shared.Common.Logging;

namespace WebAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IGenericInterface<User, Guid, UserDbContext> _generic;
        private readonly ICustomLogger<UserRepository> _logger;

        public UserRepository(IGenericInterface<User, Guid, UserDbContext> generic, ICustomLogger<UserRepository> logger)
        {
            _generic = generic ?? throw new ArgumentNullException(nameof(generic));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    _logger.Error("CreateUser request is null.");
                    throw new ArgumentNullException(nameof(user), "User data cannot be null.");
                }
                await _generic.AddAsync(user);

                return user;
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating and storing user into database.", ex);
                throw;
            }
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _generic.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.Warn("User with ID: {id} not found.", id);
                    throw new KeyNotFoundException($"User with ID: {id} not found.");
                }

                _logger.Info("User with ID: {ID} retrieved successfully.", id);
                return user!;
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving user with ID: {ID}", ex, id);
                throw;
            }
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                await _generic.UpdateAsync(user);
                _logger.Info("User with ID: {Id} updated successfully.", user.Id);

                return user;
            }
            catch (Exception ex)
            {
                _logger.Error("Error updating user.", ex);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            try
            {
                await _generic.SoftDeleteByIdAsync(id);
                _logger.Info("User with ID: {ID} deleted successfully.", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Error deleting user with ID: {ID}", ex, id);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _generic.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving all users.", ex);
                throw;
            }
        }
    }
}
