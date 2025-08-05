using Microsoft.EntityFrameworkCore;
using UrlShortener.UrlService.Data;
using UrlShortener.UrlService.Models;
using UrlShortener.Contracts.Dtos;
using UrlShortener.Common.Helpers;
using UrlShortener.Common.Services;

namespace UrlShortener.UrlService.Services;

public class UrlService : IUrlService
{
    private readonly UrlDbContext _context;
    private readonly ILogger<UrlService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ICacheService _cacheService;
    private const int CacheExpirationMinutes = 60;

    public UrlService(UrlDbContext context, ILogger<UrlService> logger, IConfiguration configuration, ICacheService cacheService)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        _cacheService = cacheService;
    }

    public async Task<ShortenedUrlResponse> CreateShortenedUrlAsync(CreateShortenedUrlRequest request)
    {
        // Validate URL
        if (!Uri.TryCreate(request.OriginalUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Invalid URL format");
        }

        // Check if URL already exists
        var existingUrl = await _context.ShortenedUrls
            .FirstOrDefaultAsync(u => u.OriginalUrl == request.OriginalUrl && u.IsActive);

        if (existingUrl != null)
        {
            return MapToResponse(existingUrl);
        }

        // Generate unique short code
        string shortCode;
        if (!string.IsNullOrEmpty(request.CustomShortCode))
        {
            if (await _context.ShortenedUrls.AnyAsync(u => u.ShortCode == request.CustomShortCode))
            {
                throw new ArgumentException("Custom short code already exists");
            }
            shortCode = request.CustomShortCode;
        }
        else
        {
            do
            {
                shortCode = ShortCodeGenerator.GenerateShortCode();
            } while (await _context.ShortenedUrls.AnyAsync(u => u.ShortCode == shortCode));
        }

        var shortenedUrl = new ShortenedUrl
        {
            ShortCode = shortCode,
            OriginalUrl = request.OriginalUrl,
            CreatedAt = DateTime.UtcNow,
            ExpirationDate = request.ExpirationDate,
            IsActive = true,
            ClickCount = 0
        };

        _context.ShortenedUrls.Add(shortenedUrl);
        await _context.SaveChangesAsync();

        // Cache the new URL
        var cacheKey = $"url:{shortCode}";
        var urlData = new CachedUrlData
        {
            OriginalUrl = request.OriginalUrl,
            ExpirationDate = request.ExpirationDate
        };
        await _cacheService.SetAsync(cacheKey, urlData, TimeSpan.FromMinutes(CacheExpirationMinutes));

        _logger.LogInformation("Created and cached shortened URL: {ShortCode} -> {OriginalUrl}", shortCode, request.OriginalUrl);

        return MapToResponse(shortenedUrl);
    }

    public async Task<string?> GetOriginalUrlAsync(string shortCode)
    {
        // First check cache
        var cacheKey = $"url:{shortCode}";
        var cachedUrl = await _cacheService.GetAsync<CachedUrlData>(cacheKey);
        
        if (cachedUrl != null)
        {
            // Check if expired
            if (cachedUrl.ExpirationDate.HasValue && cachedUrl.ExpirationDate.Value < DateTime.UtcNow)
            {
                await _cacheService.RemoveAsync(cacheKey);
                return null;
            }
            
            _logger.LogInformation("Cache hit for short code: {ShortCode}", shortCode);
            return cachedUrl.OriginalUrl;
        }
        
        // Cache miss - check database
        _logger.LogInformation("Cache miss for short code: {ShortCode}, checking database", shortCode);
        var shortenedUrl = await _context.ShortenedUrls
            .FirstOrDefaultAsync(u => u.ShortCode == shortCode && u.IsActive);

        if (shortenedUrl == null)
            return null;

        // Check if expired
        if (shortenedUrl.ExpirationDate.HasValue && shortenedUrl.ExpirationDate.Value < DateTime.UtcNow)
        {
            return null;
        }

        // Cache the result
        var urlData = new CachedUrlData
        {
            OriginalUrl = shortenedUrl.OriginalUrl,
            ExpirationDate = shortenedUrl.ExpirationDate
        };
        
        await _cacheService.SetAsync(cacheKey, urlData, TimeSpan.FromMinutes(CacheExpirationMinutes));
        _logger.LogInformation("Cached URL data for short code: {ShortCode}", shortCode);

        return shortenedUrl.OriginalUrl;
    }

    public async Task TrackClickAsync(string shortCode, HttpContext context)
    {
        var shortenedUrl = await _context.ShortenedUrls
            .FirstOrDefaultAsync(u => u.ShortCode == shortCode && u.IsActive);

        if (shortenedUrl != null)
        {
            shortenedUrl.ClickCount++;
            shortenedUrl.LastClickedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Tracked click for {ShortCode}, total clicks: {ClickCount}", 
                shortCode, shortenedUrl.ClickCount);
        }
    }

    private ShortenedUrlResponse MapToResponse(ShortenedUrl url)
    {
        var baseUrl = _configuration["BaseUrl"] ?? "http://localhost:5000";
        
        return new ShortenedUrlResponse
        {
            ShortCode = url.ShortCode,
            OriginalUrl = url.OriginalUrl,
            ShortenedUrl = $"{baseUrl}/r/{url.ShortCode}",
            CreatedAt = url.CreatedAt,
            ExpirationDate = url.ExpirationDate,
            ClickCount = url.ClickCount
        };
    }
}
