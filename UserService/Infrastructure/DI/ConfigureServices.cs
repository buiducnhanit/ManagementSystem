using Infrastructure.Data;
using ManagementSystem.Shared.Common.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Database Contexts,JWT Authentication and Serilog for logging
            services.AddSharedServices<UserDbContext>(configuration, "DefaultConnection");

            // Add other infrastructure services here

            return services;
        }
    }
}
