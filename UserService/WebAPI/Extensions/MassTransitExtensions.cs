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

                x.UsingInMemory();

                x.AddRider(rider =>
                {
                    rider.AddProducer<UserDeletedEvent>("user-topic");
                    rider.AddConsumer<UserRegisteredConsumer>();

                    rider.UsingKafka((context, cfg) =>
                    {
                        cfg.Host(configuration["Kafka:BootstrapServers"]);
                        cfg.TopicEndpoint<UserRegisteredEvent>("user-topic", nameof(UserRegisteredConsumer),e =>
                        {
                            e.ConfigureConsumer<UserRegisteredConsumer>(context);
                        });
                    });
                });
            });

            return services;
        }
    }
}
