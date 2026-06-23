using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DddCqrsJwtDemo.Application.Abstractions;
using DddCqrsJwtDemo.Application.Orders.DTOs;
using DddCqrsJwtDemo.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddCqrsJwtDemo.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetOrdersQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.Id.HasValue)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .AsQueryable();

        // Customer can only view their own orders
        if (_currentUser.Role == UserRole.Customer)
        {
            var customerId = _currentUser.Id.Value;
            query = query.Where(o => o.CustomerId == customerId);
        }

        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);

        // Fetch customer names and book titles to populate the DTOs
        var customerIds = orders.Select(o => o.CustomerId).Distinct().ToList();
        var customers = await _context.Users
            .AsNoTracking()
            .Where(u => customerIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

        var bookIds = orders.SelectMany(o => o.Items).Select(i => i.BookId).Distinct().ToList();
        var books = await _context.Books
            .AsNoTracking()
            .Where(b => bookIds.Contains(b.Id))
            .ToDictionaryAsync(b => b.Id, b => b.Title, cancellationToken);

        return orders.Select(o =>
        {
            var customerName = customers.TryGetValue(o.CustomerId, out var name) ? name : "Unknown Customer";
            var currency = o.Items.FirstOrDefault()?.Price.Currency ?? "USD";

            var itemsDto = o.Items.Select(item =>
            {
                var bookTitle = books.TryGetValue(item.BookId, out var title) ? title : "Unknown Book";
                return new OrderItemDto(
                    item.BookId,
                    bookTitle,
                    item.Quantity,
                    item.Price.Amount,
                    item.TotalPrice.Amount
                );
            }).ToList();

            return new OrderDto(
                o.Id,
                o.CustomerId,
                customerName,
                o.OrderDate,
                o.Status.ToString(),
                o.ShippingAddress.ToString(),
                itemsDto,
                o.TotalPrice.Amount,
                currency
            );
        }).ToList();
    }
}
