using CleanArchitectureDemo.Domain.Enums;
using MediatR;

namespace CleanArchitectureDemo.Application.Books.Commands.CreateBook;

public record CreateBookCommand : IRequest<Guid>
{
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Isbn { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public BookCategory Category { get; init; }
}
