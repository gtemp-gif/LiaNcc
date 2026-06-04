using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using LiaNcc.WebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LiaNcc.WebAPI.Middleware
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggingMiddleware> _logger;

        public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers["X-Correlation-ID"].ToString();
            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }
            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            try
            {
                await _next(context);

                if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 500)
                {
                    var applicationLogger = context.RequestServices.GetRequiredService<IApplicationLoggerService>();
                    await applicationLogger.LogWarningAsync("HTTP", "ClientError",
                        $"Client error occurred: {context.Response.StatusCode}",
                        null, null, new { Path = context.Request.Path.Value, Method = context.Request.Method });
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var applicationLogger = context.RequestServices.GetRequiredService<IApplicationLoggerService>();
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();

            await applicationLogger.LogCriticalAsync("Global", "UnhandledException",
                "An unhandled exception occurred in the WebAPI.",
                exception, (int)HttpStatusCode.InternalServerError);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                message = "Si è verificato un errore inatteso nel server.",
                correlationId = correlationId
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
