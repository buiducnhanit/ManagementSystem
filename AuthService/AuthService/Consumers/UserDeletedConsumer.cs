using AuthService.Entities;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Consumers
{
    public class UserDeletedConsumer : IConsumer<UserDeletedEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICustomLogger<UserDeletedConsumer> _logger;

        public UserDeletedConsumer(UserManager<ApplicationUser> userManager, ICustomLogger<UserDeletedConsumer> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserDeletedEvent> context)
        {
            var message = context.Message;
            _logger.Info("Received UserDeletedEvent for UserId: {UserId}, Email: {Email}", null, null, message.Id, message.Email!);
            var user = await _userManager.FindByIdAsync(message.Id.ToString());
            if (user != null)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue;
                await _userManager.UpdateAsync(user);
                _logger.Info("User {UserId} locked successfully.", null, null, message.Id);
            }
        }
    }
}
