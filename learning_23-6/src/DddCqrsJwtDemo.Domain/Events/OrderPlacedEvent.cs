using System;
using System.Collections.Generic;
using DddCqrsJwtDemo.Domain.Primitives;

namespace DddCqrsJwtDemo.Domain.Events;

public class OrderPlacedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public Guid OrderId { get; }
    public Guid CustomerId { get; }
    public IReadOnlyCollection<(Guid BookId, int Quantity)> Items { get; }

    public OrderPlacedEvent(Guid orderId, Guid customerId, IReadOnlyCollection<(Guid BookId, int Quantity)> items)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Items = items;
    }
}
