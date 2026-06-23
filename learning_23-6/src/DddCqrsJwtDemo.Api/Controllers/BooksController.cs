using System;
using System.Threading.Tasks;
using DddCqrsJwtDemo.Application.Books.Commands.CreateBook;
using DddCqrsJwtDemo.Application.Books.Queries.GetBookById;
using DddCqrsJwtDemo.Application.Books.Queries.GetBooks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DddCqrsJwtDemo.Api.Controllers;

public class BooksController : ApiController
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? searchTerm)
    {
        var result = await Mediator.Send(new GetBooksQuery(searchTerm));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetBookByIdQuery(id));
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateBookCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, new { Id = result });
    }
}
