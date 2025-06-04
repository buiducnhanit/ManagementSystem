using Microsoft.AspNetCore.Builder;

namespace ManagementSystem.Shared.Common.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCommonMiddlewares(this IApplicationBuilder app)
        {
            return app
                .UseMiddleware<TraceIdEnricherMiddleware>()
                .UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
