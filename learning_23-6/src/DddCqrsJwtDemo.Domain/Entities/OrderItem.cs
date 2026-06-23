using System;
using DddCqrsJwtDemo.Domain.Primitives;
using DddCqrsJwtDemo.Domain.ValueObjects;

namespace DddCqrsJwtDemo.Domain.Entities;

public class OrderItem : Entity
{
    public Guid OrderId { get; private set; }
    public Guid BookId { get; private set; }
    public int Quantity { get; private set; }
    public Money Price { get; private set; } = null!;

    public Money TotalPrice => Price.Multiply(Quantity);

    private OrderItem(Guid id, Guid orderId, Guid bookId, int quantity, Money price) : base(id)
    {
        OrderId = orderId;
        BookId = bookId;
        Quantity = quantity;
        Price = price;
    }

    // EF Core constructor
    private OrderItem()
    {
    }

    public static OrderItem Create(Guid id, Guid orderId, Guid bookId, int quantity, Money price)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        if (price is null)
            throw new ArgumentNullException(nameof(price));

        return new OrderItem(id, orderId, bookId, quantity, price);
    }
}
