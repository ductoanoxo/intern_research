using CleanArchitectureDemo.Application.Books.DTOs;
using CleanArchitectureDemo.Application.Books.Commands.CreateBook;
using CleanArchitectureDemo.Application.Books.Commands.ApplyDiscount;
using CleanArchitectureDemo.Application.Books.Queries.GetBooks;
using CleanArchitectureDemo.Application.Books.Queries.GetBookById;
using CleanArchitectureDemo.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureDemo.Api.Controllers;

public class BooksController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateBookCommand command)
    {
        var bookId = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = bookId }, bookId);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAll([FromQuery] BookCategory? category)
    {
        var query = new GetBooksQuery { Category = category };
        var books = await Mediator.Send(query);
        return Ok(books);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookDto>> GetById(Guid id)
    {
        var book = await Mediator.Send(new GetBookByIdQuery(id));
        if (book == null) return NotFound();
        return Ok(book);
    }

    [HttpPost("{id:guid}/apply-discount")]
    public async Task<ActionResult> ApplyDiscount(Guid id, [FromBody] ApplyDiscountRequest request)
    {
        var command = new ApplyDiscountCommand
        {
            BookId = id,
            DiscountType = request.DiscountType,
            Value = request.Value
        };

        var result = await Mediator.Send(command);
        if (!result) return NotFound();

        return NoContent();
    }
}

public class ApplyDiscountRequest
{
    public string DiscountType { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
