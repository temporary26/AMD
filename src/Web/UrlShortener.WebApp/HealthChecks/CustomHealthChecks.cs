using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApplication1.Data;
using WebApplication1.Services;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly ApplicationDbContext _context;

        public DatabaseHealthCheck(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Try to connect to the database and perform a simple query
                await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
                
                // Check if we can query the main tables
                var urlCount = await _context.ShortenedUrls.CountAsync(cancellationToken);
                
                var data = new Dictionary<string, object>
                {
                    { "TotalUrls", urlCount },
                    { "DatabaseProvider", _context.Database.ProviderName ?? "Unknown" }
                };

                return HealthCheckResult.Healthy("Database is healthy", data);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database is unhealthy", ex);
            }
        }
    }

    public class UrlShortenerServiceHealthCheck : IHealthCheck
    {
        private readonly IServiceProvider _serviceProvider;

        public UrlShortenerServiceHealthCheck(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var urlService = scope.ServiceProvider.GetRequiredService<IUrlShortenerService>();
                
                // Test the service by generating a short code
                var testCode = urlService.GenerateShortCode();
                
                if (string.IsNullOrEmpty(testCode))
                {
                    return Task.FromResult(HealthCheckResult.Degraded("URL Shortener service is not functioning properly"));
                }

                var data = new Dictionary<string, object>
                {
                    { "ServiceStatus", "Operational" },
                    { "TestCodeGenerated", testCode }
                };

                return Task.FromResult(HealthCheckResult.Healthy("URL Shortener service is healthy", data));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("URL Shortener service is unhealthy", ex));
            }
        }
    }
}
