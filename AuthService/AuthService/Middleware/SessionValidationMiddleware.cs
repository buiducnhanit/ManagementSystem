using AuthService.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AuthService.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SessionValidationMiddleware> _logger;

        public SessionValidationMiddleware(RequestDelegate next, ILogger<SessionValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var jwtStamp = context.User.FindFirst("AspNet.Identity.SecurityStamp")?.Value;

                _logger.LogInformation("SessionValidation: UserId from token = {UserId}", userId);
                _logger.LogInformation("SessionValidation: SecurityStamp from token = {JwtStamp}", jwtStamp);

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(jwtStamp))
                {
                    _logger.LogWarning("SessionValidation: Missing claims in token.");
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid session: missing claims.");
                    return;
                }

                var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("SessionValidation: User not found in DB.");
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid session: user not found.");
                    return;
                }

                var currentStamp = await userManager.GetSecurityStampAsync(user);

                _logger.LogInformation("SessionValidation: Current SecurityStamp from DB = {CurrentStamp}", currentStamp);

                if (jwtStamp != currentStamp)
                {
                    _logger.LogWarning("SessionValidation: SecurityStamp mismatch. Token: {JwtStamp}, DB: {CurrentStamp}", jwtStamp, currentStamp);
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid session.");
                    return;
                }

                _logger.LogInformation("SessionValidation: Session is valid.");
            }

            await _next(context);
        }
    }
}
