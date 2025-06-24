using ManagementSystem.Shared.Contracts;
using MassTransit;
using WebAPI.Consumers;

namespace WebAPI.Extensions
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMassTransitService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserRegisteredConsumer>();
                x.AddConsumer<UnLockUserConsumer>();

                x.UsingInMemory();

                x.AddRider(rider =>
                {
                    rider.AddProducer<UserDeletedEvent>("user-deleted-topic");

                    rider.AddConsumer<UserRegisteredConsumer>();
                    rider.AddConsumer<UnLockUserConsumer>();

                    rider.UsingKafka((context, cfg) =>
                    {
                        cfg.Host(configuration["Kafka:BootstrapServers"]);
                        cfg.TopicEndpoint<UserRegisteredEvent>("user-registered-topic", nameof(UserRegisteredConsumer),e =>
                        {
                            e.ConfigureConsumer<UserRegisteredConsumer>(context);
                        });

                        cfg.TopicEndpoint<UnLockUserEvent>("user-unclock-topic", nameof(UnLockUserConsumer), e =>
                        {
                            e.ConfigureConsumer<UnLockUserConsumer>(context);
                        });
                    });
                });
            });

            return services;
        }
    }
}
