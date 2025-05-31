using Microsoft.EntityFrameworkCore;
using SacraScriptura.Admin.Domain.Bibles;
using SacraScriptura.Admin.Domain.Books;
using SacraScriptura.Admin.Domain.Divisions;

namespace SacraScriptura.Admin.Infrastructure.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Bible> Bibles { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Division> Divisions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entity mappings here
        modelBuilder.Entity<Bible>(entity =>
        {
            entity.ToTable("bibles");

            entity.HasKey(e => e.Id);

            entity
                .Property(e => e.Id)
                .HasConversion(id => id!.Value, value => new BibleId(value))
                .IsRequired()
                .HasColumnName("id")
                .HasMaxLength(18)
                .IsUnicode(false)
                .UseCollation("C");

            entity
                .Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(63)
                .IsUnicode();

            entity
                .Property(e => e.LanguageCode)
                .IsRequired()
                .HasColumnName("language_code")
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("C");

            entity
                .Property(e => e.Version)
                .IsRequired()
                .HasColumnName("version")
                .HasMaxLength(63)
                .IsUnicode();

            entity.Property(e => e.Description).HasColumnName("description").IsUnicode();

            entity
                .Property(e => e.PublisherName)
                .HasColumnName("publisher_name")
                .HasMaxLength(63)
                .IsUnicode();

            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasIndex(e => new { e.Name, e.Version }).IsUnique();
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("books");

            entity.HasKey(e => e.Id);

            entity
                .Property(e => e.Id)
                .HasConversion(id => id!.Value, value => new BookId(value))
                .IsRequired()
                .HasColumnName("id")
                .HasMaxLength(18)
                .IsUnicode(false)
                .UseCollation("C");

            entity
                .Property(e => e.BibleId)
                .HasConversion(id => id!.Value, value => new BibleId(value))
                .IsRequired()
                .HasColumnName("bible_id")
                .HasMaxLength(18)
                .IsUnicode(false)
                .UseCollation("C");

            entity
                .Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(63)
                .IsUnicode();

            entity
                .Property(e => e.ShortName)
                .IsRequired()
                .HasColumnName("short_name")
                .HasMaxLength(15)
                .IsUnicode();

            entity.Property(e => e.Position).IsRequired().HasColumnName("position");

            entity.HasIndex(e => new { e.BibleId, e.Position }).IsUnique();

            entity.HasIndex(e => new { e.BibleId, e.Name }).IsUnique();

            entity
                .HasOne<Bible>()
                .WithMany()
                .HasForeignKey(e => e.BibleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Division>(entity =>
        {
            entity.ToTable("divisions");

            entity.HasKey(e => e.Id);

            entity
                .Property(e => e.Id)
                .HasConversion(id => id!.Value, value => new DivisionId(value))
                .IsRequired()
                .HasColumnName("id")
                .HasMaxLength(18)
                .IsUnicode(false)
                .UseCollation("C");

            entity
                .Property(e => e.BookId)
                .HasConversion(id => id!.Value, value => new BookId(value))
                .IsRequired()
                .HasColumnName("book_id")
                .HasMaxLength(18)
                .IsUnicode(false)
                .UseCollation("C");

            entity
                .Property(e => e.ParentId)
                .HasConversion(id => id!.Value, value => new DivisionId(value))
                .IsRequired(false)
                .HasColumnName("parent_id")
                .HasMaxLength(18)
                .IsUnicode(false)
                .UseCollation("C");

            entity.Property(e => e.Order).IsRequired().HasColumnName("order");

            entity
                .Property(e => e.Title)
                .IsRequired()
                .HasColumnName("title")
                .HasMaxLength(255)
                .IsUnicode();

            entity.Property(e => e.LeftValue).IsRequired().HasColumnName("left_value");

            entity.Property(e => e.RightValue).IsRequired().HasColumnName("right_value");

            entity.Property(e => e.Depth).IsRequired().HasColumnName("depth");

            entity.HasIndex(e => new { e.BookId, e.LeftValue });
            entity.HasIndex(e => new { e.BookId, e.RightValue });
            entity.HasIndex(e => new { e.BookId, e.Depth });

            entity
                .HasOne<Book>()
                .WithMany()
                .HasForeignKey(e => e.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne<Division>()
                .WithMany()
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Ignore(e => e.Parent);
            entity.Ignore(e => e.Children);
            entity.Ignore(e => e.Descendants);
        });
    }
}
