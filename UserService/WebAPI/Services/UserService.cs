using AutoMapper;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Contracts;
using MassTransit;
using WebAPI.DTOs;
using WebAPI.Entities;
using WebAPI.Interfaces;

namespace WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomLogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly ITopicProducer<UserDeletedEvent> _topicProducer;
        private readonly ITopicProducer<UpdateAuthEvent> _updateAuthTopicProducer;

        public UserService(IUserRepository userRepository, ICustomLogger<UserService> logger, IMapper mapper,
            ITopicProducer<UserDeletedEvent> topicProducer, ITopicProducer<UpdateAuthEvent> updateAuthTopicProducer)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _topicProducer = topicProducer ?? throw new ArgumentNullException(nameof(topicProducer));
            _updateAuthTopicProducer = updateAuthTopicProducer;
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
                    return null!;
                }

                _logger.Info("User profile created successfully.", null, null, createdUserProfile);
                return _mapper.Map<User, UserProfile>(createdUserProfile);
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating user.", ex);
                throw;
            }
        }

        public async Task<UserProfile?> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user != null)
                {
                    _logger.Info("User: {ID} retrieved successfully.", propertyValues: user.Id);
                    var userProfile = _mapper.Map<User, UserProfile>(user);
                    return userProfile;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving user with ID: {ID}", ex, propertyValues: id);
                throw;
            }
        }

        public async Task<UserProfile> UpdateUserAsync(Guid id, UpdateUserRequest request)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByIdAsync(id);
                var oldEmail = existingUser.Email;
                var oldPhoneNumber = existingUser.PhoneNumber;

                var profileUserUpdated = _mapper.Map<UpdateUserRequest, User>(request, existingUser);
                _logger.Debug("Information user update {user}", propertyValues: profileUserUpdated);

                var updatedUser = await _userRepository.UpdateUserAsync(profileUserUpdated);
                _logger.Info("User profile updated successfully.", null, null, updatedUser);

                bool isEmailChanged = oldEmail != request.Email && !string.IsNullOrEmpty(request.Email);
                bool isPhoneNumberChanged = oldPhoneNumber != request.PhoneNumber && !string.IsNullOrEmpty(request.PhoneNumber);
                if (isEmailChanged || isPhoneNumberChanged)
                {
                    await _updateAuthTopicProducer.Produce(new UpdateAuthEvent
                    {
                        Id = id,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber
                    });
                }

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
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.Warn("User with ID: {ID} not found for deletion.", null, null, id);
                    return false;
                }
                var result = await _userRepository.DeleteUserAsync(id);
                if (result)
                {
                    await _topicProducer.Produce(new UserDeletedEvent
                    {
                        Id = user.Id,
                        Email = user.Email
                    });
                    _logger.Info("UserDeletedEvent published for user ID: {ID}", null, null, id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("Error deleting user with ID: {ID}", ex, null, null, id);
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

        public async Task<bool> UnLockUserAsync(Guid id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user == null)
                {
                    throw new HandleException("User not found.", StatusCodes.Status404NotFound);
                }

                user.IsDeleted = false;
                await _userRepository.UpdateUserAsync(user);
                _logger.Info("User with ID: {ID} unlocked successfully.", propertyValues: id);
                return true;
            }
            catch (HandleException)
            {
                _logger.Error("HandleException occurred while unlocking user with ID: {ID}", propertyValues: id);
                throw;
            }
            catch (Exception ex) { throw new Exception("Error occurring unlock user.", ex); }
        }
    }
}
