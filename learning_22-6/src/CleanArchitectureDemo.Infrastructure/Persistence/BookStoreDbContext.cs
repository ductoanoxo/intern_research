using CleanArchitectureDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureDemo.Infrastructure.Persistence;

public class BookStoreDbContext : DbContext
{
    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Book entity mapping
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Author).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Isbn).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Stock).IsRequired();
            
            // Map BookCategory enum to string in DB
            entity.Property(e => e.Category)
                .HasConversion<string>()
                .HasMaxLength(50);
        });
    }
}
