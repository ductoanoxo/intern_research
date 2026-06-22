using CleanArchitectureDemo.Domain.Enums;
using CleanArchitectureDemo.Domain.Strategies;

namespace CleanArchitectureDemo.Domain.Entities;

/// <summary>
/// Domain Entity representing a Book in the bookstore.
/// Demonstrates:
/// - Single Responsibility Principle (SRP): Focuses strictly on book data and domain logic.
/// </summary>
public class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public string Isbn { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public BookCategory Category { get; private set; }

    // EF Core requires a parameterless constructor
    private Book() { }

    public Book(Guid id, string title, string author, string isbn, decimal price, int stock, BookCategory category)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Author cannot be empty.", nameof(author));
        if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentException("ISBN cannot be empty.", nameof(isbn));
        if (price < 0) throw new ArgumentException("Price cannot be negative.", nameof(price));
        if (stock < 0) throw new ArgumentException("Stock cannot be negative.", nameof(stock));

        Id = id;
        Title = title;
        Author = author;
        Isbn = isbn;
        Price = price;
        Stock = stock;
        Category = category;
    }

    public void UpdateDetails(string title, string author, string isbn, decimal price, BookCategory category)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Author cannot be empty.", nameof(author));
        if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentException("ISBN cannot be empty.", nameof(isbn));
        if (price < 0) throw new ArgumentException("Price cannot be negative.", nameof(price));

        Title = title;
        Author = author;
        Isbn = isbn;
        Price = price;
        Category = category;
    }

    public void UpdateStock(int changeQuantity)
    {
        int newStock = Stock + changeQuantity;
        if (newStock < 0)
        {
            throw new InvalidOperationException($"Insufficient stock. Current stock is {Stock}, requested change is {changeQuantity}.");
        }
        Stock = newStock;
    }

    /// <summary>
    /// Applies a discount strategy to calculate the discounted price and update the Price property.
    /// Demonstrates:
    /// - Dependency Inversion Principle (DIP): The entity depends on the abstraction IBookDiscountStrategy.
    /// </summary>
    public void ApplyDiscount(IBookDiscountStrategy discountStrategy)
    {
        ArgumentNullException.ThrowIfNull(discountStrategy);
        Price = discountStrategy.ApplyDiscount(Price);
    }
}
