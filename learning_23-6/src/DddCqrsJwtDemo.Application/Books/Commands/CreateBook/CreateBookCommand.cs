using System;
using MediatR;

namespace DddCqrsJwtDemo.Application.Books.Commands.CreateBook;

public record CreateBookCommand(
    string Title,
    string Author,
    decimal Price,
    string Currency,
    int Stock) : IRequest<Guid>;
