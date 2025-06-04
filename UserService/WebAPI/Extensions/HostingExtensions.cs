using Infrastructure.DI;
using ManagementSystem.Shared.Common.DependencyInjection;

namespace WebAPI.Extensions
{
    public static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddInfrastructureServices(builder.Configuration)
                            .AddWebAPIService();

            return builder;
        }

        public static WebApplication ConfigureMiddlewares(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseCustomSwagger();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseSharedPolicies();

            return app;
        }
    }
}
