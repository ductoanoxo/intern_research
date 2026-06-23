using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DddCqrsJwtDemo.Application.Abstractions;
using DddCqrsJwtDemo.Domain.Entities;
using DddCqrsJwtDemo.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddCqrsJwtDemo.Application.Orders.Commands.PlaceOrder;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public PlaceOrderCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.Id.HasValue)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var customerId = _currentUser.Id.Value;

        // Build shipping address value object
        var shippingAddress = Address.Create(request.Street, request.City, request.ZipCode);

        // Build Aggregate Root
        var order = Order.Create(Guid.NewGuid(), customerId, shippingAddress);

        // Add items to order
        foreach (var itemCommand in request.Items)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == itemCommand.BookId, cancellationToken);

            if (book is null)
            {
                throw new ArgumentException($"Book with ID '{itemCommand.BookId}' was not found.");
            }

            // Enforce domain invariant: check stock here or in event handler.
            // Under DDD, checking stock inside Book aggregate is best practice.
            // Let's check stock immediately to give immediate feedback.
            if (book.Stock < itemCommand.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for book '{book.Title}'. Available: {book.Stock}, requested: {itemCommand.Quantity}");
            }

            order.AddItem(book.Id, itemCommand.Quantity, book.Price);
        }

        // Place order (triggers domain event OrderPlacedEvent)
        order.Place();

        _context.Orders.Add(order);

        // SaveChangesAsync will trigger the event dispatcher, executing OrderPlacedEventHandler
        // which will decrement the book stock, and save everything together in one database transaction!
        await _context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
