using Microsoft.AspNetCore.Builder;
using Serilog;

namespace ManagementSystem.Shared.Common.Logging
{
    public static class SerilogConfiguration
    {
        public static void ConfigureSerilog(ConfigureHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((ctx, lc) => lc
                .ReadFrom.Configuration(ctx.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            );
        }
    }
}
