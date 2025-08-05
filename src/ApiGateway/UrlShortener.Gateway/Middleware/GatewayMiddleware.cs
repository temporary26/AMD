using System.Diagnostics;

namespace UrlShortener.Gateway.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();
            
            // Add request ID to response headers
            context.Response.Headers["X-Request-ID"] = requestId;
            
            // Log request start
            _logger.LogInformation("Gateway Request Start: {RequestId} {Method} {Path} from {RemoteIP}",
                requestId,
                context.Request.Method,
                context.Request.Path,
                GetClientIpAddress(context));

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                
                // Log request completion
                _logger.LogInformation("Gateway Request Complete: {RequestId} {Method} {Path} -> {StatusCode} in {ElapsedMs}ms",
                    requestId,
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
        }

        private static string GetClientIpAddress(HttpContext context)
        {
            // Check for forwarded IP first (in case of proxy/load balancer)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }

    public class GatewayErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GatewayErrorHandlingMiddleware> _logger;

        public GatewayErrorHandlingMiddleware(RequestDelegate next, ILogger<GatewayErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gateway Error: {Method} {Path} from {RemoteIP}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Connection.RemoteIpAddress?.ToString());

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    error = "Gateway Error",
                    message = "An error occurred while processing your request through the gateway",
                    timestamp = DateTime.UtcNow,
                    path = context.Request.Path.Value,
                    requestId = context.Response.Headers["X-Request-ID"].FirstOrDefault()
                };

                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
            }
        }
    }
}
