using AuthService.Consumers;
using ManagementSystem.Shared.Contracts;
using MassTransit;

namespace AuthService.Extensions
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMassTransitService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserDeletedConsumer>();

                x.UsingInMemory();
               
                x.AddRider(rider =>
                {
                    rider.AddProducer<UserRegisteredEvent>("user-topic");
                    rider.AddConsumer<UserDeletedConsumer>();
                    rider.UsingKafka((context, cfg) =>
                    {
                        cfg.Host(configuration["Kafka:BootstrapServers"]);
                        cfg.TopicEndpoint<UserDeletedEvent>("user-topic", nameof(UserDeletedConsumer), e =>
                        {
                            e.ConfigureConsumer<UserDeletedConsumer>(context);
                        });
                    });
                });
            });

            return services;
        }
    }
}
