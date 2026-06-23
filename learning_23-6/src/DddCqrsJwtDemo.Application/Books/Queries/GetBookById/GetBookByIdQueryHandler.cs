using System.Threading;
using System.Threading.Tasks;
using DddCqrsJwtDemo.Application.Abstractions;
using DddCqrsJwtDemo.Application.Books.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddCqrsJwtDemo.Application.Books.Queries.GetBookById;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private readonly IApplicationDbContext _context;

    public GetBookByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (book is null)
        {
            return null;
        }

        return new BookDto(
            book.Id,
            book.Title,
            book.Author,
            book.Price.Amount,
            book.Price.Currency,
            book.Stock
        );
    }
}
