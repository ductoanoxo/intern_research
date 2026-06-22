using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    // Resolve MediatR Sender lazily from HTTP context services
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
