namespace UrlShortener.Contracts.Dtos;

public class CreateShortenedUrlRequest
{
    public string OriginalUrl { get; set; } = string.Empty;
    public string? CustomShortCode { get; set; }
    public DateTime? ExpirationDate { get; set; }
}

public class ShortenedUrlResponse
{
    public string ShortCode { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortenedUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public int ClickCount { get; set; }
}

public class UrlStatsResponse
{
    public string ShortCode { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public int TotalClicks { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastClickedAt { get; set; }
}
