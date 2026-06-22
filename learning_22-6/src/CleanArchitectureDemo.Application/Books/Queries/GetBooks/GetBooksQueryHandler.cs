using CleanArchitectureDemo.Application.Books.DTOs;
using CleanArchitectureDemo.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureDemo.Application.Books.Queries.GetBooks;

public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, IEnumerable<BookDto>>
{
    private readonly IBookRepository _bookRepository;

    public GetBooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetAllAsync(cancellationToken);

        if (request.Category.HasValue)
        {
            books = books.Where(b => b.Category == request.Category.Value);
        }

        return books.Select(b => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Isbn = b.Isbn,
            Price = b.Price,
            Stock = b.Stock,
            Category = b.Category.ToString()
        });
    }
}
