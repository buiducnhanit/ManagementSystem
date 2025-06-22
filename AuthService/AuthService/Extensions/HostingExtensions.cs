using Asp.Versioning.ApiExplorer;
using AuthService.Configure;
using AuthService.Data;
using AuthService.Interfaces;
using AuthService.Repositories;
using AuthService.Services;
using ManagementSystem.Shared.Common.DependencyInjection;
using ManagementSystem.Shared.Common.Logging;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Extensions
{
    public static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Host.ConfigureSerilog();

            // Configure Entity Framework Core with SQL Server
            builder.Services.AddDbContextService<IdentityDbContext>(configuration, "DefaultConnection");

            // Configure Identity services
            builder.Services.AddIdentityService(configuration);

            builder.Services.AddJWTAuthenticationScheme(configuration);

            // Add Serilog for logging
            builder.Services.AddScoped(typeof(ICustomLogger<>), typeof(SerilogCustomLogger<>));

            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IAuthService, AuthServices>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddHostedService<TokenCleanupService>();

            // Configure email services
            builder.Services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<ISendMailService, SendMailService>();

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(5);
            });

            // Add MassTransit for Kafka
            builder.Services.AddMassTransitService(configuration);

            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddSwaggerExtension();
            builder.Services.AddHttpClient();

            return builder;
        }

        public static async Task<WebApplication> ConfigureMiddlewaresAsync(this WebApplication app)
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await SeedData.InitializeAsync(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ICustomLogger<Program>>();
                    logger.Error("An error occurred while initializing the database.", ex);
                    throw;
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerExtension(provider);
            }

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseSharedPolicies();

            return app;
        }
    }
}
