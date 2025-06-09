namespace WebAPI.Extensions
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddCustomCors();
            services.AddCustomPolicies();
            services.AddCustomSwagger();
            services.AddHttpClient();

            return services;
        }
    }
}
    