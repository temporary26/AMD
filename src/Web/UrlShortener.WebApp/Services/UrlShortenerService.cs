using Microsoft.EntityFrameworkCore;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UrlShortenerService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICachingService _cachingService;
        private readonly Random _random = new();
        private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public UrlShortenerService(ApplicationDbContext context, ILogger<UrlShortenerService> logger, 
            IHttpContextAccessor httpContextAccessor, ICachingService cachingService)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _cachingService = cachingService;
        }

        public async Task<ShortenedUrlResponse> CreateShortenedUrlAsync(CreateShortenedUrlRequest request, string? userId = null)
        {
            try
            {
                // Validate URL
                if (!Uri.TryCreate(request.OriginalUrl, UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                {
                    throw new ArgumentException("Invalid URL format");
                }

                // Check if URL already exists for this user
                var existingUrl = await _context.ShortenedUrls
                    .FirstOrDefaultAsync(u => u.OriginalUrl == request.OriginalUrl && 
                                            u.UserId == userId && 
                                            u.IsActive);

                if (existingUrl != null)
                {
                    return MapToResponse(existingUrl);
                }

                // Generate unique short code
                string shortCode;
                if (!string.IsNullOrWhiteSpace(request.CustomCode))
                {
                    shortCode = request.CustomCode;
                    // Check if custom code is available
                    if (await _context.ShortenedUrls.AnyAsync(u => u.ShortCode == shortCode))
                    {
                        throw new ArgumentException("Custom code is already in use");
                    }
                }
                else
                {
                    shortCode = await GenerateUniqueShortCodeAsync();
                }

                var shortenedUrl = new ShortenedUrl
                {
                    OriginalUrl = request.OriginalUrl,
                    ShortCode = shortCode,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = request.ExpiresAt,
                    UserId = userId,
                    IsActive = true
                };

                _context.ShortenedUrls.Add(shortenedUrl);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created shortened URL {ShortCode} for {OriginalUrl}", shortCode, request.OriginalUrl);

                return MapToResponse(shortenedUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shortened URL for {OriginalUrl}", request.OriginalUrl);
                throw;
            }
        }

        public async Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode)
        {
            try
            {
                var cacheKey = $"url:{shortCode}";
                
                // Try to get from cache first
                var cachedUrl = await _cachingService.GetAsync<ShortenedUrl>(cacheKey);
                if (cachedUrl != null)
                {
                    // Check if URL has expired
                    if (cachedUrl.ExpiresAt.HasValue && cachedUrl.ExpiresAt.Value <= DateTime.UtcNow)
                    {
                        cachedUrl.IsActive = false;
                        await _context.SaveChangesAsync();
                        await _cachingService.RemoveAsync(cacheKey);
                        return null;
                    }
                    return cachedUrl;
                }

                // Get from database
                var url = await _context.ShortenedUrls
                    .FirstOrDefaultAsync(u => u.ShortCode == shortCode && u.IsActive);

                if (url != null)
                {
                    // Check if URL has expired
                    if (url.ExpiresAt.HasValue && url.ExpiresAt.Value <= DateTime.UtcNow)
                    {
                        url.IsActive = false;
                        await _context.SaveChangesAsync();
                        return null;
                    }

                    // Cache for 30 minutes
                    await _cachingService.SetAsync(cacheKey, url, TimeSpan.FromMinutes(30));
                }

                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving URL by short code {ShortCode}", shortCode);
                return null;
            }
        }

        public async Task<UrlStatisticsResponse?> GetUrlStatisticsAsync(string shortCode)
        {
            try
            {
                var url = await _context.ShortenedUrls
                    .Include(u => u.UrlClicks.OrderByDescending(c => c.ClickedAt).Take(100))
                    .FirstOrDefaultAsync(u => u.ShortCode == shortCode);

                if (url == null) return null;

                var baseUrl = GetBaseUrl();
                url.ShortUrl = $"{baseUrl}/{url.ShortCode}";

                return new UrlStatisticsResponse
                {
                    Id = url.Id,
                    OriginalUrl = url.OriginalUrl,
                    ShortCode = url.ShortCode,
                    ShortUrl = url.ShortUrl,
                    CreatedAt = url.CreatedAt,
                    ExpiresAt = url.ExpiresAt,
                    TotalClicks = url.ClickCount,
                    IsActive = url.IsActive,
                    RecentClicks = url.UrlClicks.Select(c => new ClickStatistic
                    {
                        ClickedAt = c.ClickedAt,
                        IpAddress = c.IpAddress,
                        UserAgent = c.UserAgent,
                        Country = c.Country,
                        City = c.City
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics for {ShortCode}", shortCode);
                return null;
            }
        }

        public async Task<List<ShortenedUrlResponse>> GetUserUrlsAsync(string userId)
        {
            try
            {
                var urls = await _context.ShortenedUrls
                    .Where(u => u.UserId == userId)
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();

                return urls.Select(MapToResponse).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving URLs for user {UserId}", userId);
                return new List<ShortenedUrlResponse>();
            }
        }

        public async Task<bool> RecordClickAsync(string shortCode, string? ipAddress, string? userAgent, string? referer)
        {
            try
            {
                var url = await _context.ShortenedUrls
                    .FirstOrDefaultAsync(u => u.ShortCode == shortCode);

                if (url == null) return false;

                // Increment click count
                url.ClickCount++;

                // Record click details
                var click = new UrlClick
                {
                    ShortenedUrlId = url.Id,
                    ClickedAt = DateTime.UtcNow,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Referer = referer
                };

                _context.UrlClicks.Add(click);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording click for {ShortCode}", shortCode);
                return false;
            }
        }

        public async Task<bool> DeactivateUrlAsync(string shortCode, string? userId = null)
        {
            try
            {
                var url = await _context.ShortenedUrls
                    .FirstOrDefaultAsync(u => u.ShortCode == shortCode && 
                                            (userId == null || u.UserId == userId));

                if (url == null) return false;

                url.IsActive = false;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating URL {ShortCode}", shortCode);
                return false;
            }
        }

        public string GenerateShortCode(int length = 6)
        {
            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(Characters[_random.Next(Characters.Length)]);
            }
            return result.ToString();
        }

        private async Task<string> GenerateUniqueShortCodeAsync(int length = 6, int maxAttempts = 10)
        {
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var shortCode = GenerateShortCode(length);
                
                if (!await _context.ShortenedUrls.AnyAsync(u => u.ShortCode == shortCode))
                {
                    return shortCode;
                }
                
                // If we have collisions, try with longer codes
                if (attempt == maxAttempts / 2)
                {
                    length++;
                }
            }

            throw new InvalidOperationException("Unable to generate unique short code after multiple attempts");
        }

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                return "https://localhost:5199"; // Fallback for background processes
            }

            var scheme = request.Scheme;
            var host = request.Host.Value;
            return $"{scheme}://{host}";
        }

        private ShortenedUrlResponse MapToResponse(ShortenedUrl url)
        {
            var baseUrl = GetBaseUrl();
            url.ShortUrl = $"{baseUrl}/{url.ShortCode}";
            
            return new ShortenedUrlResponse
            {
                Id = url.Id,
                OriginalUrl = url.OriginalUrl,
                ShortCode = url.ShortCode,
                ShortUrl = url.ShortUrl,
                CreatedAt = url.CreatedAt,
                ExpiresAt = url.ExpiresAt,
                ClickCount = url.ClickCount,
                IsActive = url.IsActive
            };
        }
    }
}
