using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services
{
    public interface IUrlShortenerService
    {
        Task<ShortenedUrlResponse> CreateShortenedUrlAsync(CreateShortenedUrlRequest request, string? userId = null);
        Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode);
        Task<UrlStatisticsResponse?> GetUrlStatisticsAsync(string shortCode);
        Task<List<ShortenedUrlResponse>> GetUserUrlsAsync(string userId);
        Task<bool> RecordClickAsync(string shortCode, string? ipAddress, string? userAgent, string? referer);
        Task<bool> DeactivateUrlAsync(string shortCode, string? userId = null);
        string GenerateShortCode(int length = 6);
    }
}
