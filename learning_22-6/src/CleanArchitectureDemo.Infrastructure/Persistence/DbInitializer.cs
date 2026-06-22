using CleanArchitectureDemo.Domain.Entities;
using CleanArchitectureDemo.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureDemo.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(BookStoreDbContext context)
    {
        // Automatically create SQLite database file and tables if they don't exist
        await context.Database.EnsureCreatedAsync();

        // Seed default books if empty
        if (await context.Books.AnyAsync())
        {
            return;
        }

        var books = new List<Book>
        {
            new Book(Guid.NewGuid(), "Clean Architecture", "Robert C. Martin", "978-0134494166", 35.50m, 15, BookCategory.Technology),
            new Book(Guid.NewGuid(), "Design Patterns", "Erich Gamma", "978-0201633610", 49.99m, 8, BookCategory.Technology),
            new Book(Guid.NewGuid(), "The Hobbit", "J.R.R. Tolkien", "978-0261102217", 12.99m, 25, BookCategory.Fiction),
            new Book(Guid.NewGuid(), "A Brief History of Time", "Stephen Hawking", "978-0553380163", 18.00m, 12, BookCategory.Science),
            new Book(Guid.NewGuid(), "Steve Jobs", "Walter Isaacson", "978-1451648539", 21.00m, 10, BookCategory.Biography)
        };

        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();
    }
}
