using System.Text.Json;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Response;
using Microsoft.AspNetCore.Http;

namespace ManagementSystem.Shared.Common.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HandleException ex)
            {
                await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
            }
            catch (Exception)
            {
                await HandleExceptionAsync(context, 500, "Internal Server Error");
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var traceId = context.TraceIdentifier;
            var response = ApiResponse<string>.FailureResponse(message, statusCode, null, traceId);
            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
