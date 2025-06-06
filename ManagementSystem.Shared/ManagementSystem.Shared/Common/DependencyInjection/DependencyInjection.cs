using ManagementSystem.Shared.Common.Interfaces;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Common.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ManagementSystem.Shared.Common.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedServices<TDbContext>(this IServiceCollection services, IConfiguration configuration, string connectionStringName) where TDbContext : DbContext
        {
            // Add DbContext service
            services.AddDbContextService<TDbContext>(configuration, connectionStringName);

            // Add JWT authentication scheme
            services.AddJWTAuthenticationScheme(configuration);

            // Add Serilog for logging
            services.AddScoped(typeof(ICustomLogger<>), typeof(SerilogCustomLogger<>));
            services.AddScoped(typeof(IGenericInterface<,,>), typeof(GenericInterface<,,>));

            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            app.UseCommonMiddlewares();
            app.UseSerilogRequestLogging();

            return app;
        }

        public static void ConfigureSerilog(this ConfigureHostBuilder hostBuilder)
        {
            SerilogConfiguration.ConfigureSerilog(hostBuilder);
        }
    }
}
