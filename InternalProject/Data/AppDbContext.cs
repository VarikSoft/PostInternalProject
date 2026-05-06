using Microsoft.EntityFrameworkCore;
using InternalProject.Domain;

namespace InternalProject.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts => Set<Post>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(builder =>
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Body)
                .IsRequired();

            builder.Property(p => p.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(p => p.Version)
                .IsConcurrencyToken()
                .IsRequired();

            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.AuthorId);
            builder.HasIndex(p => p.PublishedAt);
        });
    }
}
