using System;
using DddCqrsJwtDemo.Application.Books.DTOs;
using MediatR;

namespace DddCqrsJwtDemo.Application.Books.Queries.GetBookById;

public record GetBookByIdQuery(Guid Id) : IRequest<BookDto?>;
