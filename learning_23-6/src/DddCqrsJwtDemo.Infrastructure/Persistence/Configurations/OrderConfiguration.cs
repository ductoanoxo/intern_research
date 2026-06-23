using DddCqrsJwtDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DddCqrsJwtDemo.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.CustomerId)
            .IsRequired();

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<int>()
            .IsRequired();

        // OwnsOne for ShippingAddress Value Object
        builder.OwnsOne(o => o.ShippingAddress, addr =>
        {
            addr.Property(a => a.Street)
                .HasColumnName("ShippingStreet")
                .HasMaxLength(150)
                .IsRequired();

            addr.Property(a => a.City)
                .HasColumnName("ShippingCity")
                .HasMaxLength(100)
                .IsRequired();

            addr.Property(a => a.ZipCode)
                .HasColumnName("ShippingZipCode")
                .HasMaxLength(20)
                .IsRequired();
        });

        // Setup relationship with OrderItem
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Tell EF Core to access Items via private field backing field
        builder.Navigation(o => o.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
