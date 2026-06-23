using System;
using System.Linq;
using System.Threading.Tasks;
using DddCqrsJwtDemo.Application.Abstractions;
using DddCqrsJwtDemo.Domain.Entities;
using DddCqrsJwtDemo.Domain.Enums;
using DddCqrsJwtDemo.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DddCqrsJwtDemo.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        // Automatically create DB and apply schema
        await context.Database.EnsureCreatedAsync();

        // Check if DB is already seeded
        if (await context.Users.AnyAsync() || await context.Books.AnyAsync())
        {
            return;
        }

        // 1. Seed Users
        var adminUser = User.Create(
            Guid.NewGuid(),
            Email.Create("admin@bookstore.com"),
            passwordHasher.Hash("adminpassword"),
            "System Administrator",
            UserRole.Admin
        );

        var customerUser = User.Create(
            Guid.NewGuid(),
            Email.Create("customer@bookstore.com"),
            passwordHasher.Hash("customerpassword"),
            "John Doe (Customer)",
            UserRole.Customer
        );

        await context.Users.AddRangeAsync(adminUser, customerUser);

        // 2. Seed Books
        var books = new[]
        {
            Book.Create(
                Guid.Parse("3f6933a3-35f1-4e75-bacb-05e3c59ec312"),
                "Clean Architecture",
                "Robert C. Martin",
                Money.Create(35.50m, "USD"),
                15
            ),
            Book.Create(
                Guid.Parse("b4c4ad32-1185-4c35-8952-bd3934988921"),
                "Domain-Driven Design",
                "Eric Evans",
                Money.Create(49.99m, "USD"),
                5
            ),
            Book.Create(
                Guid.Parse("c5d5ad32-2285-4c35-8952-bd3934988922"),
                "Refactoring",
                "Martin Fowler",
                Money.Create(42.00m, "USD"),
                8
            )
        };

        await context.Books.AddRangeAsync(books);

        // Save seeds
        await context.SaveChangesAsync();
    }
}
