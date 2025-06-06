namespace WebAPI.Extensions
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization();
            services.AddControllers();
            services.AddSwaggerExtension();
            services.AddHttpClient();

            return services;
        }
    }
}
