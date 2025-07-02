using AutoMapper;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Contracts;
using MassTransit;
using WebAPI.DTOs;
using WebAPI.Interfaces;

namespace WebAPI.Consumers
{
    public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
    {
        private readonly IUserService _userService;
        private readonly ICustomLogger<UserRegisteredConsumer> _logger;
        private readonly IMapper _mapper;

        public UserRegisteredConsumer(IUserService userService, ICustomLogger<UserRegisteredConsumer> logger, IMapper mapper)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            try
            {
                var UserRegisteredEvent = context.Message;
                _logger.Debug("User registered event data message: {UserRegisteredEvent}", propertyValues: UserRegisteredEvent);
                if(await _userService.GetUserByIdAsync(UserRegisteredEvent.Id) != null)
                {
                    _logger.Warn("User with ID: {ID} already exists.", propertyValues: UserRegisteredEvent.Id);
                    return;
                }
                var userProfile = await _userService.CreateUserAsync(_mapper.Map<CreateUserRequest>(UserRegisteredEvent));
                _logger.Info("User registered successfully.", null, null, userProfile);
            }
            catch (HandleException hex)
            {
                _logger.Error("HandleException occurred while consuming UserRegisteredEvent.", hex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Error consuming UserRegisteredEvent.", ex);
                throw;
            }
        }
    }
}
