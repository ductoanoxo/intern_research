using CleanArchitectureDemo.Domain.Enums;

namespace CleanArchitectureDemo.Application.Books.DTOs;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
}
