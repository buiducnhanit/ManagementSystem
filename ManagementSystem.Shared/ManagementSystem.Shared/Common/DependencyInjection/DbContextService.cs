using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManagementSystem.Shared.Common.DependencyInjection
{
    public static class DbContextService
    {
        public static IServiceCollection AddDbContextService<TDbContext>(this IServiceCollection services, IConfiguration configuration, string connectionStringName) where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString(connectionStringName)));

            return services;
        }
    }
}
