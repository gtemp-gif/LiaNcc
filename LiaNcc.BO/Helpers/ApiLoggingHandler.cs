using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LiaNcc.BO.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LiaNcc.BO.Helpers
{
    public class ApiLoggingHandler : DelegatingHandler
    {
        private readonly IApplicationLoggerService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiLoggingHandler(IApplicationLoggerService logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Evita loop infiniti se la chiamata è verso l'endpoint dei log
            if (request.RequestUri != null && request.RequestUri.AbsolutePath.Contains("/api/logs", System.StringComparison.OrdinalIgnoreCase))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var stopwatch = Stopwatch.StartNew();
            var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString();

            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                stopwatch.Stop();

                if (!response.IsSuccessStatusCode)
                {
                    await _logger.LogWarningAsync(
                        "API",
                        "OutgoingRequest",
                        $"API call to {request.Method} {request.RequestUri} failed with status {response.StatusCode}",
                        null,
                        "ExternalAPI",
                        null,
                        "ApiResponseError",
                        new
                        {
                            Url = request.RequestUri?.ToString(),
                            Method = request.Method.ToString(),
                            StatusCode = (int)response.StatusCode,
                            DurationMs = stopwatch.ElapsedMilliseconds,
                            CorrelationId = correlationId
                        });
                }

                return response;
            }
            catch (System.Exception ex)
            {
                stopwatch.Stop();
                await _logger.LogErrorAsync(
                    "API",
                    "OutgoingRequest",
                    $"API call to {request.Method} {request.RequestUri} failed with exception",
                    ex,
                    null,
                    null,
                    "ExternalAPI",
                    null,
                    "ApiResponseException",
                    new
                    {
                        Url = request.RequestUri?.ToString(),
                        Method = request.Method.ToString(),
                        DurationMs = stopwatch.ElapsedMilliseconds,
                        CorrelationId = correlationId
                    });
                throw;
            }
        }
    }
}
