using DddCqrsJwtDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DddCqrsJwtDemo.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Id)
            .ValueGeneratedNever();

        builder.Property(oi => oi.OrderId)
            .IsRequired();

        builder.Property(oi => oi.BookId)
            .IsRequired();

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        // OwnsOne for Price Value Object
        builder.OwnsOne(oi => oi.Price, price =>
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
    }
}
