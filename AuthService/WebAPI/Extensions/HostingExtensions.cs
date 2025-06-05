using Asp.Versioning.ApiExplorer;
using Infrastructure.Data;
using Infrastructure.DependencyInjection;
using ManagementSystem.Shared.Common.DependencyInjection;
using ManagementSystem.Shared.Common.Logging;

namespace WebAPI.Extensions
{
    public static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Host.ConfigureSerilog();
            builder.Services.AddInfrastructureService(configuration);
            builder.Services.AddWebApiServices(configuration);

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

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseInfrastructurePolicy();

            return app;
        }
    }
}
