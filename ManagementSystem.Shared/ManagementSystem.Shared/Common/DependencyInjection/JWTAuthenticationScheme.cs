using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ManagementSystem.Shared.Common.DependencyInjection
{
    public static class JWTAuthenticationScheme
    {
        public static IServiceCollection AddJWTAuthenticationScheme<TUser, TKey>(this IServiceCollection services, IConfiguration configuration) 
            where TUser : IdentityUser<TKey> 
            where TKey : IEquatable<TKey>
        {
            // Add authentication and JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<TUser>>();
                            var userPrincipal = context.Principal;

                            var isValid = await signInManager.ValidateSecurityStampAsync(userPrincipal!);
                            if (isValid == null)
                            {
                                context.Fail("Invalid security stamp.");
                                return;
                            }
                        },
                    };
                });

            return services;
        }
    }
}
