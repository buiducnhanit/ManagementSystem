using AuthService.Interfaces;

namespace AuthService.Services
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenCleanupService> _logger;
        private TimeSpan _interval;
        private int _timeoutHours;

        public TokenCleanupService(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<TokenCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;

            _interval = TimeSpan.FromMinutes(_configuration.GetValue<int>("Session:CleanupIntervalMinutes", 10));
            _timeoutHours = _configuration.GetValue<int>("Session:IdleSessionTimeoutInHours", 1);

            _logger.LogInformation($"TokenCleanupService initialized. Cleanup Interval: {_interval.TotalMinutes} minutes. Idle Timeout: {_timeoutHours} hours.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TokenCleanupService has started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var refreshTokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

                        _logger.LogInformation($"Attempting to revoke tokens inactive for {_timeoutHours} hours at {DateTimeOffset.Now}.");
                        await refreshTokenRepository.RevokeTokensInactiveFor(TimeSpan.FromHours(_timeoutHours));
                        _logger.LogInformation("Token cleanup completed for this cycle.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while revoking inactive tokens in TokenCleanupService.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("TokenCleanupService has stopped.");
        }
    }
}