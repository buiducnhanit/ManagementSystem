using Microsoft.AspNetCore.Http;

namespace ManagementSystem.Shared.Common.Middleware
{
    public class TraceIdEnricherMiddleware
    {
        private readonly RequestDelegate _next;

        public TraceIdEnricherMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var traceId = context.TraceIdentifier;
            using (Serilog.Context.LogContext.PushProperty("TraceId", traceId))
            {
                await _next(context);
            }
        }
    }
}
