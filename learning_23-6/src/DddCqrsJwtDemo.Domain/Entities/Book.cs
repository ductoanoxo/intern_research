using System;
using DddCqrsJwtDemo.Domain.Events;
using DddCqrsJwtDemo.Domain.Primitives;
using DddCqrsJwtDemo.Domain.ValueObjects;

namespace DddCqrsJwtDemo.Domain.Entities;

public class Book : AggregateRoot
{
    public string Title { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public int Stock { get; private set; }

    private Book(Guid id, string title, string author, Money price, int stock) : base(id)
    {
        Title = title;
        Author = author;
        Price = price;
        Stock = stock;
    }

    // EF Core constructor
    private Book()
    {
    }

    public static Book Create(Guid id, string title, string author, Money price, int stock)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author is required.", nameof(author));
        if (stock < 0)
            throw new ArgumentException("Stock cannot be negative.", nameof(stock));

        return new Book(id, title.Trim(), author.Trim(), price, stock);
    }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice is null)
        {
            throw new ArgumentNullException(nameof(newPrice));
        }

        if (Price != newPrice)
        {
            var oldPrice = Price;
            Price = newPrice;
            RaiseDomainEvent(new BookPriceChangedEvent(Id, oldPrice, newPrice));
        }
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity to decrease must be positive.", nameof(quantity));
        }

        if (Stock < quantity)
        {
            throw new InvalidOperationException($"Insufficient stock for book '{Title}'. Available: {Stock}, requested: {quantity}");
        }

        Stock -= quantity;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity to increase must be positive.", nameof(quantity));
        }

        Stock += quantity;
    }
}
