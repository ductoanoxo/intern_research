using CleanArchitectureDemo.Application.Books.DTOs;
using CleanArchitectureDemo.Domain.Enums;
using MediatR;

namespace CleanArchitectureDemo.Application.Books.Queries.GetBooks;

public record GetBooksQuery : IRequest<IEnumerable<BookDto>>
{
    public BookCategory? Category { get; init; }
}
