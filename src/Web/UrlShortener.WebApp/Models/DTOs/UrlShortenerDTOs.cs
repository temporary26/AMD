using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTOs
{
    public class CreateShortenedUrlRequest
    {
        [Required]
        [Url]
        [MaxLength(2048)]
        public string OriginalUrl { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? CustomCode { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }

    public class ShortenedUrlResponse
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int ClickCount { get; set; }
        public bool IsActive { get; set; }
    }

    public class UrlStatisticsResponse
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int TotalClicks { get; set; }
        public bool IsActive { get; set; }
        public List<ClickStatistic> RecentClicks { get; set; } = new List<ClickStatistic>();
    }

    public class ClickStatistic
    {
        public DateTime ClickedAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
    }
}
