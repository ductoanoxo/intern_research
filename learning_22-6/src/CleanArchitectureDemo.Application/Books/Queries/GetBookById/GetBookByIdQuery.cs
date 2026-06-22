using CleanArchitectureDemo.Application.Books.DTOs;
using MediatR;

namespace CleanArchitectureDemo.Application.Books.Queries.GetBookById;

public record GetBookByIdQuery(Guid Id) : IRequest<BookDto?>;
