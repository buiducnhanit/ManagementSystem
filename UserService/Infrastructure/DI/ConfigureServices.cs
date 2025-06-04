using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext with SQL Server provider
            services.AddDbContext(configuration);

            // Register JWT authentication scheme
            services.AddJWTAuthentication(configuration);

            return services;
        }
    }
}
