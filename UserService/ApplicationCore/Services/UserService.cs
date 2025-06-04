using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ManagementSystem.Shared.Common.Logging;

namespace ApplicationCore.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomLogger<UserService> _logger;
        public UserService(IUserRepository userRepository, ICustomLogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserProfile> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                var newUserProfile = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Address = request.Address,
                    AvatarUrl = request.AvatarUrl,
                    //PhoneNumber = request.PhoneNumber,
                    //DateOfBirth = request.DateOfBirth
                };

                var createdUserProfile = await _userRepository.CreateUserAsync(newUserProfile);
                if (createdUserProfile == null)
                {
                    _logger.Error("Failed to create user profile.");
                    throw new Exception("Failed to create user profile.");
                }

                _logger.Info("User profile created successfully.", createdUserProfile);
                return new UserProfile
                {
                    Id = createdUserProfile.Id,
                    FirstName = createdUserProfile.FirstName,
                    LastName = createdUserProfile.LastName,
                    Address = createdUserProfile.Address,
                    AvatarUrl = createdUserProfile.AvatarUrl,
                    //PhoneNumber = createdUserProfile.PhoneNumber,
                    //DateOfBirth = createdUserProfile.DateOfBirth
                };
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating user.", ex);
                throw;
            }
        }
        public Task<UserProfile> GetUserByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public Task<UserProfile> UpdateUserAsync(UpdateUserRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserProfile>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
