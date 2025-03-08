using Microsoft.EntityFrameworkCore;
using SacraScriptura.Domain.Bibles;

namespace SacraScriptura.Infrastructure.Database;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options
) : DbContext(options)
{
    public DbSet<Bible> Bibles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entity mappings here
        modelBuilder.Entity<Bible>(
            entity =>
            {
                entity.ToTable("bibles");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .HasConversion(
                          id => id!.Value,
                          value => new BibleId(value)
                      )
                      .IsRequired()
                      .HasColumnName("id")
                      .HasMaxLength(18)
                      .IsUnicode(false)
                      .UseCollation("C");

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasColumnName("name")
                      .HasMaxLength(63)
                      .IsUnicode();

                entity.Property(e => e.LanguageCode)
                      .IsRequired()
                      .HasColumnName("language_code")
                      .HasMaxLength(5)
                      .IsUnicode(false)
                      .UseCollation("C");

                entity.Property(e => e.Version)
                      .IsRequired()
                      .HasColumnName("version")
                      .HasMaxLength(63)
                      .IsUnicode();

                entity.Property(e => e.Description)
                      .HasColumnName("description")
                      .IsUnicode();

                entity.Property(e => e.PublisherName)
                      .HasColumnName("publisher_name")
                      .HasMaxLength(63)
                      .IsUnicode();

                entity.Property(e => e.Year)
                      .HasColumnName("year");

                entity.HasIndex(
                          e => new
                          {
                              e.Name,
                              e.Version
                          }
                      )
                      .IsUnique();
            }
        );
    }
}