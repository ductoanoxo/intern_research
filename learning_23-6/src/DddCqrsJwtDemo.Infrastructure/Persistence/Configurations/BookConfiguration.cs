using DddCqrsJwtDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DddCqrsJwtDemo.Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .ValueGeneratedNever();

        builder.Property(b => b.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Author)
            .HasMaxLength(100)
            .IsRequired();

        // OwnsOne for Price Value Object
        builder.OwnsOne(b => b.Price, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("PriceAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            price.Property(p => p.Currency)
                .HasColumnName("PriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(b => b.Stock)
            .IsRequired();
    }
}
