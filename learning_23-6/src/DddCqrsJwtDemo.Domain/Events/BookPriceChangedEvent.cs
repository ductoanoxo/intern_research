using System;
using DddCqrsJwtDemo.Domain.Primitives;
using DddCqrsJwtDemo.Domain.ValueObjects;

namespace DddCqrsJwtDemo.Domain.Events;

public class BookPriceChangedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public Guid BookId { get; }
    public Money OldPrice { get; }
    public Money NewPrice { get; }

    public BookPriceChangedEvent(Guid bookId, Money oldPrice, Money newPrice)
    {
        BookId = bookId;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}
