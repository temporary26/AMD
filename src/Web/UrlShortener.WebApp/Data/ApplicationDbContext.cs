using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
        public DbSet<UrlClick> UrlClicks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ShortenedUrl entity
            modelBuilder.Entity<ShortenedUrl>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OriginalUrl).IsRequired().HasMaxLength(2048);
                entity.Property(e => e.ShortCode).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.ShortCode).IsUnique();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ClickCount).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                
                // Create index for faster lookups
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.IsActive);
            });

            // Configure UrlClick entity
            modelBuilder.Entity<UrlClick>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClickedAt).IsRequired();
                entity.Property(e => e.IpAddress).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.Referer).HasMaxLength(500);
                entity.Property(e => e.Country).HasMaxLength(100);
                entity.Property(e => e.City).HasMaxLength(100);

                // Configure relationship
                entity.HasOne(e => e.ShortenedUrl)
                      .WithMany(e => e.UrlClicks)
                      .HasForeignKey(e => e.ShortenedUrlId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Create indexes for analytics queries
                entity.HasIndex(e => e.ShortenedUrlId);
                entity.HasIndex(e => e.ClickedAt);
                entity.HasIndex(e => new { e.ShortenedUrlId, e.ClickedAt });
            });
        }
    }
}
