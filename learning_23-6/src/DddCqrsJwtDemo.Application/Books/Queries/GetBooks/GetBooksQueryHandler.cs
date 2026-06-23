using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DddCqrsJwtDemo.Application.Abstractions;
using DddCqrsJwtDemo.Application.Books.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddCqrsJwtDemo.Application.Books.Queries.GetBooks;

public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, List<BookDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBooksQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(searchTerm) || b.Author.ToLower().Contains(searchTerm));
        }

        var books = await query.ToListAsync(cancellationToken);

        return books.Select(b => new BookDto(
            b.Id,
            b.Title,
            b.Author,
            b.Price.Amount,
            b.Price.Currency,
            b.Stock
        )).ToList();
    }
}
