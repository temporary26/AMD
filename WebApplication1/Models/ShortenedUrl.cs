using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class ShortenedUrl
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(2048)]
        public string OriginalUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string ShortCode { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public int ClickCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        // Optional: Associate with user if they want to track their URLs
        public string? UserId { get; set; }

        // Navigation property for click tracking
        public virtual ICollection<UrlClick> UrlClicks { get; set; } = new List<UrlClick>();

        // Helper property to get the full short URL - will be set by service
        [NotMapped]
        public string ShortUrl { get; set; } = string.Empty;
    }
}
