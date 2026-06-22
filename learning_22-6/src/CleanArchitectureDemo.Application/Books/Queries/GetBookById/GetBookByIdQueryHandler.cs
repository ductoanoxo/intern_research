using CleanArchitectureDemo.Application.Books.DTOs;
using CleanArchitectureDemo.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureDemo.Application.Books.Queries.GetBookById;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private readonly IBookRepository _bookRepository;

    public GetBookByIdQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken);
        if (book == null) return null;

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Isbn = book.Isbn,
            Price = book.Price,
            Stock = book.Stock,
            Category = book.Category.ToString()
        };
    }
}
