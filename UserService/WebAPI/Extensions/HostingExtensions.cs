using Asp.Versioning.ApiExplorer;
using Infrastructure.DI;
using ManagementSystem.Shared.Common.DependencyInjection;

namespace WebAPI.Extensions
{
    public static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Host.ConfigureSerilog();
            builder.Services.AddInfrastructureServices(configuration)
                            .AddWebAPIService();

            return builder;
        }

        public static WebApplication ConfigureMiddlewares(this WebApplication app)
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            if (app.Environment.IsDevelopment())
            {
                app.UseCustomSwagger(provider);
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseSharedPolicies();

            return app;
        }
    }
}
