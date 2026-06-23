using System;
using System.Collections.Generic;
using System.Linq;
using DddCqrsJwtDemo.Domain.Enums;
using DddCqrsJwtDemo.Domain.Events;
using DddCqrsJwtDemo.Domain.Primitives;
using DddCqrsJwtDemo.Domain.ValueObjects;

namespace DddCqrsJwtDemo.Domain.Entities;

public class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = new();

    public Guid CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public Address ShippingAddress { get; private set; } = null!;
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public Money TotalPrice
    {
        get
        {
            if (!_items.Any())
            {
                return Money.Zero();
            }

            var currency = _items.First().Price.Currency;
            var totalAmount = _items.Sum(x => x.TotalPrice.Amount);
            return Money.Create(totalAmount, currency);
        }
    }

    private Order(Guid id, Guid customerId, Address shippingAddress) : base(id)
    {
        CustomerId = customerId;
        ShippingAddress = shippingAddress;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }

    // EF Core constructor
    private Order()
    {
    }

    public static Order Create(Guid id, Guid customerId, Address shippingAddress)
    {
        return new Order(id, customerId, shippingAddress);
    }

    public void AddItem(Guid bookId, int quantity, Money price)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Cannot add items to a non-pending order.");
        }

        if (_items.Any() && _items.First().Price.Currency != price.Currency)
        {
            throw new InvalidOperationException("Cannot add items of different currencies to the same order.");
        }

        var orderItemId = Guid.NewGuid();
        var orderItem = OrderItem.Create(orderItemId, Id, bookId, quantity, price);
        _items.Add(orderItem);
    }

    public void Place()
    {
        if (!_items.Any())
        {
            throw new InvalidOperationException("Cannot place an order with no items.");
        }

        // Raise the OrderPlacedEvent so stock can be decreased
        var eventItems = _items.Select(x => (x.BookId, x.Quantity)).ToList();
        RaiseDomainEvent(new OrderPlacedEvent(Id, CustomerId, eventItems));
    }

    public void Complete()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Only pending orders can be completed.");
        }

        Status = OrderStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed)
        {
            throw new InvalidOperationException("Completed orders cannot be cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }
}
