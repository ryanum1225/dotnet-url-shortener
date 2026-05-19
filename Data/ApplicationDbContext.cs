using dotnet_url_shortener.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_url_shortener.Data
{
    public class ShardAContext : DbContext
    {
        public ShardAContext(DbContextOptions<ShardAContext> options) : base(options)
        {
            
        }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortenedUrl>(builder =>
            {
                builder
                    .Property(shortenedUrl => shortenedUrl.Code)
                    .HasMaxLength(ShortLinkSettings.Length);

                builder
                    .HasIndex(shortenedUrl => shortenedUrl.Code)
                    .IsUnique();
            });
        }

    }

    public class ShardBContext : DbContext
    {
        public ShardBContext(DbContextOptions<ShardBContext> options) : base(options)
        {

        }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortenedUrl>(builder =>
            {
                builder
                    .Property(shortenedUrl => shortenedUrl.Code)
                    .HasMaxLength(ShortLinkSettings.Length);

                builder
                    .HasIndex(shortenedUrl => shortenedUrl.Code)
                    .IsUnique();
            });
        }
    }
}
