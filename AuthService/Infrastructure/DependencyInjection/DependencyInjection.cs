using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Infrastructure.ExtendedServices.Email;
using Infrastructure.ExtendedServices.JWT;
using Infrastructure.Identity;
using Infrastructure.JWT;
using ManagementSystem.Shared.Common.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSharedServices<IdentityDbContext, ApplicationUser, IdentityRole<Guid>, Guid>(configuration, "DefaultConnection");

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                // Cấu hình mật khẩu
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Cấu hình lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình người dùng
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;

                // Cấu hình xác thực email
                options.SignIn.RequireConfirmedEmail = false;
            })
                    .AddRoles<IdentityRole<Guid>>()
                    .AddEntityFrameworkStores<IdentityDbContext>()
                    .AddDefaultTokenProviders();

            services.AddHttpContextAccessor();
            services.AddScoped<SignInManager<ApplicationUser>>();
            services.AddScoped<RoleManager<IdentityRole<Guid>>>();
            services.AddScoped<SecurityStampValidator<ApplicationUser>>();
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(0);
            });

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            // Configure email services
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddTransient<ISendMailService, SendMailService>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            app.UseSharedPolicies();

            return app;
        }
    }
}
