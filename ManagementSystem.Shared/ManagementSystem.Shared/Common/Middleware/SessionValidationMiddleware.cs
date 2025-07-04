using ManagementSystem.Shared.Common.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace ManagementSystem.Shared.Common.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;
        private readonly IServiceProvider _serviceProvider;

        public SessionValidationMiddleware(RequestDelegate next, IDistributedCache cache, IServiceProvider serviceProvider)
        {
            _next = next;
            _cache = cache;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var scope = _serviceProvider.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ICustomLogger<SessionValidationMiddleware>>();
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var jwtStamp = context.User.FindFirst("AspNet.Identity.SecurityStamp")?.Value;

                var stampInCache = await _cache.GetStringAsync($"session:{userId}");
                if (stampInCache == null || stampInCache != jwtStamp)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("{\"error\":\"InvalidSession\"}");
                    return;
                }
            }

            await _next(context);
        }
    }
}
