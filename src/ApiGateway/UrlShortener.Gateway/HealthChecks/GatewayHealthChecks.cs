using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UrlShortener.Gateway.HealthChecks
{
    public class DownstreamServiceHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DownstreamServiceHealthCheck> _logger;

        public DownstreamServiceHealthCheck(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<DownstreamServiceHealthCheck> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var baseUrl = _configuration["DownstreamServices:UrlShortenerAPI:BaseUrl"];
                var healthPath = _configuration["DownstreamServices:UrlShortenerAPI:HealthCheckPath"];
                var timeoutSeconds = _configuration.GetValue<int>("DownstreamServices:UrlShortenerAPI:TimeoutSeconds", 30);

                if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(healthPath))
                {
                    return HealthCheckResult.Unhealthy("Configuration for downstream service is missing");
                }

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

                var healthCheckUrl = $"{baseUrl.TrimEnd('/')}{healthPath}";
                var response = await httpClient.GetAsync(healthCheckUrl, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    
                    var data = new Dictionary<string, object>
                    {
                        { "ServiceUrl", baseUrl },
                        { "HealthCheckUrl", healthCheckUrl },
                        { "StatusCode", (int)response.StatusCode },
                        { "ResponseTime", DateTime.UtcNow }
                    };

                    return HealthCheckResult.Healthy($"Downstream service is healthy", data);
                }
                else
                {
                    var data = new Dictionary<string, object>
                    {
                        { "ServiceUrl", baseUrl },
                        { "StatusCode", (int)response.StatusCode },
                        { "ReasonPhrase", response.ReasonPhrase ?? "Unknown" }
                    };

                    return HealthCheckResult.Degraded($"Downstream service returned status code {response.StatusCode}", null, data);
                }
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogWarning("Health check timeout for downstream service");
                return HealthCheckResult.Degraded("Downstream service health check timed out");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking downstream service health");
                return HealthCheckResult.Unhealthy("Error checking downstream service health", ex);
            }
        }
    }

    public class GatewayHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, object>
            {
                { "GatewayStatus", "Running" },
                { "Version", "1.0.0" },
                { "Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown" },
                { "MachineName", Environment.MachineName },
                { "ProcessorCount", Environment.ProcessorCount },
                { "WorkingSet", GC.GetTotalMemory(false) }
            };

            return Task.FromResult(HealthCheckResult.Healthy("Gateway is healthy", data));
        }
    }
}
