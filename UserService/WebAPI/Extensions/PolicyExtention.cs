namespace WebAPI.Extensions
{
    public static class PolicyExtention
    {
        public static IServiceCollection AddCustomPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("InternalServiceCall", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("is_internal_service", "true");
                });
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            });

            return services;
        }
    }
}
