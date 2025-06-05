namespace WebAPI.Extensions
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services)
        {
            services.AddControllers();
            // Register custom CORS policy
            services.AddCustomCors();
            // Register custom authorization policies
            services.AddCustomPolicies();
            services.AddCustomSwagger();

            return services;
        }
    }
}
    