namespace AuthService.Extensions
{
    public static class ExternalAuthenticationProviders
    {
        public static IServiceCollection AddGoogleProvider(this IServiceCollection services, IConfiguration configuration)
        {
            var googleClientId = configuration["Authentication:Google:ClientId"];
            var googleClientSecret = configuration["Authentication:Google:ClientSecret"];

            if (string.IsNullOrEmpty(googleClientId) || string.IsNullOrEmpty(googleClientSecret))
            {
                throw new ArgumentException("Google ClientId and ClientSecret must be configured in the appsettings.json file.");
            }

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = googleClientId;
                    options.ClientSecret = googleClientSecret;
                    options.SaveTokens = true;
                });

            return services;
        }
    }
}
