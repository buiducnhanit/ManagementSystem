using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;

namespace ApplicationCore.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomLogger<UserService> _logger;
        private readonly IMapperService _mapper;

        public UserService(IUserRepository userRepository, ICustomLogger<UserService> logger, IMapperService mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<UserProfile> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                var userUpdated = _mapper.Map<CreateUserRequest, User>(request);
                var createdUserProfile = await _userRepository.CreateUserAsync(userUpdated);
                if (createdUserProfile == null)
                {
                    _logger.Error("Failed to create user profile.");
                    throw new Exception("Failed to create user profile.");
                }

                _logger.Info("User profile created successfully.", createdUserProfile);
                return _mapper.Map<User, UserProfile>(createdUserProfile);
            }
            catch (HandleException hex)
            {
                _logger.Error("HandleException occurred while creating user.", hex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating user.", ex);
                throw;
            }
        }

        public async Task<UserProfile> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                _logger.Info("User: {ID} retrieved successfully.", user.Id);

                var userProfile = _mapper.Map<User, UserProfile>(user);

                return userProfile;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error retrieving user with ID: {id}", ex);
                throw;
            }
        }

        public async Task<UserProfile> UpdateUserAsync(Guid id, UpdateUserRequest request)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    _logger.Warn($"User with ID: {id} not found for update.");
                    throw new KeyNotFoundException($"User with ID: {id} not found.");
                }

                var profileUserUpdated = _mapper.Map<UpdateUserRequest, User>(request);
                var updatedUser = await _userRepository.UpdateUserAsync(profileUserUpdated);
                _logger.Info("User profile updated successfully.", updatedUser);

                return _mapper.Map<User, UserProfile>(updatedUser);
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
                return await _userRepository.DeleteUserAsync(id);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting user with ID: {id}", ex);
                throw;
            }
        }

        public Task<IEnumerable<UserProfile>> GetAllUsersAsync()
        {
            try
            {
                var users = _userRepository.GetAllUsersAsync();
                _logger.Info("All users retrieved successfully.");
                return users.ContinueWith(task => task.Result.Select(user => _mapper.Map<User, UserProfile>(user)));
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving all users.", ex);
                throw;
            }
        }
    }
}
