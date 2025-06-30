using AuthService.Interfaces;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Contracts;
using MassTransit;

namespace AuthService.Consumers
{
    public class UpdateAuthConsumer : IConsumer<UpdateAuthEvent>
    {
        private readonly IAuthService _authServices;
        private readonly ICustomLogger<UpdateAuthConsumer> _logger;

        public UpdateAuthConsumer(IAuthService authServices, ICustomLogger<UpdateAuthConsumer> logger)
        {
            _authServices = authServices;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UpdateAuthEvent> context)
        {
            var message = context.Message;
            _logger.Info("Received UpdateAuthEvent for UserId: {UserId}, Email: {Email}", propertyValues: [message.Id, message.Email!]);
            await _authServices.UpdateUserInfoAsync(message);
            _logger.Info("User {UserId} updated successfully.", propertyValues: [message.Id]);
        }
    }
}
