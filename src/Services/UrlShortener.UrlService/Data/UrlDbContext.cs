using Microsoft.EntityFrameworkCore;
using UrlShortener.UrlService.Models;

namespace UrlShortener.UrlService.Data;

public class UrlDbContext : DbContext
{
    public UrlDbContext(DbContextOptions<UrlDbContext> options) : base(options)
    {
    }

    public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ShortenedUrl>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ShortCode).IsUnique();
            entity.HasIndex(e => e.OriginalUrl);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
