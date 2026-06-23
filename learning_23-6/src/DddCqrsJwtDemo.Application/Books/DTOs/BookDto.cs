using System;

namespace DddCqrsJwtDemo.Application.Books.DTOs;

public record BookDto(
    Guid Id,
    string Title,
    string Author,
    decimal Price,
    string Currency,
    int Stock);
