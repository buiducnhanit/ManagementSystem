using AutoMapper;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Contracts;
using MassTransit;
using MassTransit.Transports;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ITopicProducer<UserDeletedEvent> _topicProducer;

        public UserService(IUserRepository userRepository, ICustomLogger<UserService> logger, IMapper mapper, IHttpClientFactory httpClientFactory,
            IConfiguration configuration, ITopicProducer<UserDeletedEvent> topicProducer)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _topicProducer = topicProducer ?? throw new ArgumentNullException(nameof(topicProducer));
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

                _logger.Info("User profile created successfully.", null, null, createdUserProfile);
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
                _logger.Info("User: {ID} retrieved successfully.", null, null, user.Id);

                var userProfile = _mapper.Map<User, UserProfile>(user);

                return userProfile;
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving user with ID: {ID}", ex, null, null, id);
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
                    _logger.Warn("User with ID: {ID} not found for update.", null, null, id);
                    throw new KeyNotFoundException($"User with ID: {id} not found.");
                }

                var oldEmail = existingUser.Email;
                var oldPhoneNumber = existingUser.PhoneNumber;

                var profileUserUpdated = _mapper.Map<UpdateUserRequest, User>(request, existingUser);
                _logger.Debug("Information user update {user}", propertyValues: profileUserUpdated);
                //profileUserUpdated.Id = id;
                var updatedUser = await _userRepository.UpdateUserAsync(profileUserUpdated);
                _logger.Info("User profile updated successfully.", null, null, updatedUser);

                bool isEmailChanged = oldEmail != request.Email && !string.IsNullOrEmpty(request.Email);
                bool isPhoneNumberChanged = oldPhoneNumber != request.PhoneNumber && !string.IsNullOrEmpty(request.PhoneNumber);
                if (isEmailChanged || isPhoneNumberChanged)
                {
                    await SynchronizeAuthServiceUserInfoAsync(new UpdateAuthEvent
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

        private async Task SynchronizeAuthServiceUserInfoAsync(UpdateAuthEvent request)
        {
            _logger.Info("Attempting to synchronize user ID: {UserId} info with AuthService.", null, null, request.Id);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["AuthService:BaseUrl"]!);

            //var internalToken = _jwtInternalService.GenerateInternalServiceToken();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", internalToken);

            var updateAuthCommand = new UpdateAuthEvent
            {
                Id = request.Id,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
            };

            try
            {
                var response = await client.PutAsJsonAsync("api/v1/auth/user-info", updateAuthCommand);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.Error("Failed to update user info in AuthService.");
                    throw new HttpRequestException($"AuthService synchronization failed: {response.StatusCode} - {errorContent}");
                }
                _logger.Info("User info synchronized successfully with AuthService for user ID: {UserId}", null, null, request.Id);
            }
            catch (HttpRequestException ex)
            {
                _logger.Error("Network or HTTP error calling AuthService.");
                throw new Exception("Error during AuthService synchronization. Please contact support.", ex);
            }
            catch (Exception ex)
            {
                _logger.Error("An unexpected error occurred during AuthService.");
                throw new Exception("An unexpected error occurred during AuthService synchronization.", ex);
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
    }
}
