using CleanArchitectureDemo.Application.Common.Interfaces;
using CleanArchitectureDemo.Domain.Entities;
using MediatR;

namespace CleanArchitectureDemo.Application.Books.Commands.CreateBook;

/// <summary>
/// Handler for CreateBookCommand.
/// Demonstrates:
/// - Single Responsibility Principle (SRP): This handler is solely responsible for creating a Book.
/// </summary>
public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Guid>
{
    private readonly IBookRepository _bookRepository;

    public CreateBookCommandHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Guid> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = new Book(
            Guid.NewGuid(),
            request.Title,
            request.Author,
            request.Isbn,
            request.Price,
            request.Stock,
            request.Category
        );

        await _bookRepository.AddAsync(book, cancellationToken);

        return book.Id;
    }
}
