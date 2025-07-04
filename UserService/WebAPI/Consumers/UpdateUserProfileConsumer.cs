using AutoMapper;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Contracts;
using MassTransit;
using WebAPI.DTOs;
using WebAPI.Entities;
using WebAPI.Interfaces;

namespace WebAPI.Consumers
{
    public class UpdateUserProfileConsumer : IConsumer<UpdateUserProfileEvent>
    {
        private readonly IUserService _userService;
        private readonly ICustomLogger<UpdateUserProfileConsumer> _logger;
        private readonly IMapper _mapper;

        public UpdateUserProfileConsumer(IUserService userService, ICustomLogger<UpdateUserProfileConsumer> logger, IMapper mapper)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<UpdateUserProfileEvent> context)
        {
            try
            {
                var updateUserProfileEvent = context.Message;
                var userExists = await _userService.GetUserByIdAsync(Guid.Parse(updateUserProfileEvent.Id));
                userExists.Roles = updateUserProfileEvent.Roles;
                var updateUserRequest = _mapper.Map<UpdateUserRequest>(userExists);
                await _userService.UpdateUserAsync(Guid.Parse(context.Message.Id), updateUserRequest);
            }
            catch (HandleException hex)
            {
                _logger.Error("HandleException occurred while consuming UpdateUserProfileEvent.", hex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Error consuming UpdateUserProfileEvent.", ex);
                throw;
            }
        }
    }
}
