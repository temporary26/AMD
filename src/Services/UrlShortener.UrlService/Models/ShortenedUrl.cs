using System.ComponentModel.DataAnnotations;

namespace UrlShortener.UrlService.Models;

public class ShortenedUrl
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string ShortCode { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(2048)]
    public string OriginalUrl { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? LastClickedAt { get; set; }
    public int ClickCount { get; set; }
    public bool IsActive { get; set; } = true;
    
    public string? UserId { get; set; }
}
