using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UrlClick
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ShortenedUrlId { get; set; }

        [Required]
        public DateTime ClickedAt { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        [MaxLength(500)]
        public string? Referer { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        // Navigation property
        public virtual ShortenedUrl ShortenedUrl { get; set; } = null!;
    }
}
