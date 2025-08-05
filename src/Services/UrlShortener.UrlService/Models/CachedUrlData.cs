namespace UrlShortener.UrlService.Models;

public class CachedUrlData
{
    public string OriginalUrl { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
}
