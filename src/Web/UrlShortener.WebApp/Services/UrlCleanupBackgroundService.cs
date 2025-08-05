using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Services
{
    public class UrlCleanupBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UrlCleanupBackgroundService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1); // Run every hour

        public UrlCleanupBackgroundService(IServiceProvider serviceProvider, ILogger<UrlCleanupBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("URL Cleanup Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PerformCleanupAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during URL cleanup");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }

            _logger.LogInformation("URL Cleanup Background Service stopped");
        }

        private async Task PerformCleanupAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var cutoffDate = DateTime.UtcNow;
            
            // Deactivate expired URLs
            var expiredUrls = await context.ShortenedUrls
                .Where(u => u.ExpiresAt.HasValue && u.ExpiresAt <= cutoffDate && u.IsActive)
                .ToListAsync();

            if (expiredUrls.Any())
            {
                foreach (var url in expiredUrls)
                {
                    url.IsActive = false;
                }

                await context.SaveChangesAsync();
                _logger.LogInformation("Deactivated {Count} expired URLs", expiredUrls.Count);
            }

            // Clean up old click records (keep only last 6 months)
            var oldClicksCutoff = DateTime.UtcNow.AddMonths(-6);
            var oldClicksCount = await context.UrlClicks
                .Where(c => c.ClickedAt < oldClicksCutoff)
                .CountAsync();

            if (oldClicksCount > 0)
            {
                // Delete in batches to avoid long-running transactions
                const int batchSize = 1000;
                var totalDeleted = 0;

                while (true)
                {
                    var oldClicks = await context.UrlClicks
                        .Where(c => c.ClickedAt < oldClicksCutoff)
                        .Take(batchSize)
                        .ToListAsync();

                    if (!oldClicks.Any())
                        break;

                    context.UrlClicks.RemoveRange(oldClicks);
                    await context.SaveChangesAsync();
                    
                    totalDeleted += oldClicks.Count;
                    
                    if (oldClicks.Count < batchSize)
                        break;
                }

                _logger.LogInformation("Deleted {Count} old click records", totalDeleted);
            }

            // Delete inactive URLs older than 1 year that have no clicks
            var inactiveUrlsCutoff = DateTime.UtcNow.AddYears(-1);
            var inactiveUrls = await context.ShortenedUrls
                .Where(u => !u.IsActive && 
                           u.CreatedAt < inactiveUrlsCutoff && 
                           u.ClickCount == 0)
                .ToListAsync();

            if (inactiveUrls.Any())
            {
                context.ShortenedUrls.RemoveRange(inactiveUrls);
                await context.SaveChangesAsync();
                _logger.LogInformation("Deleted {Count} old inactive URLs with no clicks", inactiveUrls.Count);
            }

            _logger.LogDebug("URL cleanup completed successfully");
        }
    }
}
