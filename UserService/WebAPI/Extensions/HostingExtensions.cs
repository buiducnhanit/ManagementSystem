using Asp.Versioning.ApiExplorer;
using ManagementSystem.Shared.Common.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Interfaces;
using WebAPI.Repositories;
using WebAPI.Services;

namespace WebAPI.Extensions
{
    public static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Host.ConfigureSerilog();

            // Add Database Contexts,JWT Authentication and Serilog for logging
            builder.Services.AddSharedServices<UserDbContext>(configuration, "DefaultConnection");

            // Add other infrastructure services here
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();


            builder.Services.AddControllers();
            builder.Services.AddCustomCors();
            builder.Services.AddCustomPolicies();
            builder.Services.AddCustomSwagger();
            builder.Services.AddHttpClient();

            return builder;
        }

        public static WebApplication ConfigureMiddlewares(this WebApplication app)
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                db.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseCustomSwagger(provider);
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
