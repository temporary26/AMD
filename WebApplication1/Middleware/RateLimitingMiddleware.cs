using System.Collections.Concurrent;
using System.Net;

namespace WebApplication1.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clients = new();
        private readonly int _maxRequestsPerMinute;
        private readonly TimeSpan _timeWindow;

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _maxRequestsPerMinute = configuration.GetValue<int>("RateLimiting:MaxRequestsPerMinute", 60);
            _timeWindow = TimeSpan.FromMinutes(1);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = GetClientIdentifier(context);
            
            if (IsRateLimited(clientId))
            {
                _logger.LogWarning("Rate limit exceeded for client {ClientId}", clientId);
                
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.Headers["Retry-After"] = "60";
                
                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            await _next(context);
        }

        private static string GetClientIdentifier(HttpContext context)
        {
            // Try to get the real IP address (considering proxies)
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

        private bool IsRateLimited(string clientId)
        {
            var now = DateTime.UtcNow;
            
            var clientInfo = _clients.AddOrUpdate(clientId, 
                new ClientRequestInfo { RequestCount = 1, WindowStart = now },
                (key, existingInfo) =>
                {
                    // If the time window has passed, reset the counter
                    if (now - existingInfo.WindowStart > _timeWindow)
                    {
                        existingInfo.RequestCount = 1;
                        existingInfo.WindowStart = now;
                    }
                    else
                    {
                        existingInfo.RequestCount++;
                    }
                    return existingInfo;
                });

            return clientInfo.RequestCount > _maxRequestsPerMinute;
        }

        private class ClientRequestInfo
        {
            public int RequestCount { get; set; }
            public DateTime WindowStart { get; set; }
        }
    }
}
