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
                x.UsingInMemory();

                x.AddRider(rider =>
                {
                    rider.AddProducer<UserRegisteredEvent>("user-registered-topic");
                    rider.UsingKafka((context, cfg) =>
                    {
                        cfg.Host(configuration["Kafka:BootstrapServers"]);
                    });
                });
            });

            return services;
        }
    }
}
