using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Infrastructure.ExtendedServices.Email;
using Infrastructure.ExtendedServices.JWT;
using Infrastructure.Identity;
using Infrastructure.JWT;
using ManagementSystem.Shared.Common.DependencyInjection;
using ManagementSystem.Shared.Common.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Entity Framework Core with SQL Server
            services.AddDbContextService<IdentityDbContext>(configuration, "DefaultConnection");

            // Configure Identity services
            services.AddIdentityService(configuration);

            services.AddJWTAuthenticationScheme(configuration);

            // Add Serilog for logging
            services.AddScoped(typeof(ICustomLogger<>), typeof(SerilogCustomLogger<>));

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            // Configure email services
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddTransient<ISendMailService, SendMailService>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            app.UseSharedPolicies();

            return app;
        }
    }
}
