using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManagementSystem.Shared.Common.DependencyInjection
{
    public static class RedisCacheService
    {
        public static IServiceCollection AddRedisCacheService(this IServiceCollection services, IConfiguration configuration, string connectionStringName)
        {
            var redisConnectionString = configuration.GetConnectionString(connectionStringName);
            if (string.IsNullOrEmpty(redisConnectionString))
            {
                throw new ArgumentException("Redis connection string is not configured.");
            }
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "ManagementSystem:";
            });
            return services;
        }
    }
}
