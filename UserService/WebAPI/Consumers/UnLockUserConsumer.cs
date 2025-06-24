using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Contracts;
using MassTransit;
using WebAPI.Interfaces;

namespace WebAPI.Consumers
{
    public class UnLockUserConsumer : IConsumer<UnLockUserEvent>
    {
        private readonly IUserService _userService;
        private readonly ICustomLogger<UnLockUserConsumer> _logger;

        public UnLockUserConsumer(IUserService userService, ICustomLogger<UnLockUserConsumer> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UnLockUserEvent> context)
        {
            try
            {
                var userId = context.Message.Id;
                await _userService.UnLockUserAsync(Guid.Parse(userId));
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
