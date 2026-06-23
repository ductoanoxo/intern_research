using System;
using System.Threading;
using System.Threading.Tasks;
using DddCqrsJwtDemo.Application.Abstractions;
using DddCqrsJwtDemo.Domain.Entities;
using DddCqrsJwtDemo.Domain.ValueObjects;
using MediatR;

namespace DddCqrsJwtDemo.Application.Books.Commands.CreateBook;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateBookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var price = Money.Create(request.Price, request.Currency ?? "USD");
        
        var book = Book.Create(
            Guid.NewGuid(),
            request.Title,
            request.Author,
            price,
            request.Stock
        );

        _context.Books.Add(book);
        await _context.SaveChangesAsync(cancellationToken);

        return book.Id;
    }
}
