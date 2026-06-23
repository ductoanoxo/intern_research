using System.Threading.Tasks;
using DddCqrsJwtDemo.Application.Auth.Commands.Login;
using DddCqrsJwtDemo.Application.Auth.Commands.Register;
using Microsoft.AspNetCore.Mvc;

namespace DddCqrsJwtDemo.Api.Controllers;

public class AuthController : ApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(new { UserId = result });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}
