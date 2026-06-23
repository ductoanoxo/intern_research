using System.Collections.Generic;
using DddCqrsJwtDemo.Application.Books.DTOs;
using MediatR;

namespace DddCqrsJwtDemo.Application.Books.Queries.GetBooks;

public record GetBooksQuery(string? SearchTerm = null) : IRequest<List<BookDto>>;
