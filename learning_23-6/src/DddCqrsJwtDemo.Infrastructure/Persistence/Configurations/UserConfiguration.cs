using DddCqrsJwtDemo.Domain.Entities;
using DddCqrsJwtDemo.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DddCqrsJwtDemo.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        // Convert Email value object to/from string
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value))
            .HasMaxLength(256)
            .IsRequired();

        // Email should be unique
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Role)
            .HasConversion<int>()
            .IsRequired();
    }
}
