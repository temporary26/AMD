using UrlShortener.Contracts.Dtos;

namespace UrlShortener.UrlService.Services;

public interface IUrlService
{
    Task<ShortenedUrlResponse> CreateShortenedUrlAsync(CreateShortenedUrlRequest request);
    Task<string?> GetOriginalUrlAsync(string shortCode);
    Task TrackClickAsync(string shortCode, HttpContext context);
}
