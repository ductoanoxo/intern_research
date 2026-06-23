using System;
using System.Threading;
using System.Threading.Tasks;
using DddCqrsJwtDemo.Application.Abstractions;
using DddCqrsJwtDemo.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddCqrsJwtDemo.Application.Events;

public class OrderPlacedEventHandler : INotificationHandler<DomainEventNotification<OrderPlacedEvent>>
{
    private readonly IApplicationDbContext _context;

    public OrderPlacedEventHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DomainEventNotification<OrderPlacedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        foreach (var item in domainEvent.Items)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == item.BookId, cancellationToken);

            if (book is null)
            {
                throw new InvalidOperationException($"Book with ID '{item.BookId}' was not found during order processing.");
            }

            // Decrease the book stock using Domain logic (enforces invariants)
            book.DecreaseStock(item.Quantity);
        }
    }
}
