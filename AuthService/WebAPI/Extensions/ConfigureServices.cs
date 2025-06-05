using Asp.Versioning.ApiExplorer;
using ManagementSystem.Shared.Common.Logging;

namespace WebAPI.Extensions
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization();
            services.AddControllers();
            services.AddSwaggerExtension();

            return services;
        }
    }
}
