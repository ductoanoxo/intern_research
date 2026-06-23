using System.Threading.Tasks;
using DddCqrsJwtDemo.Application.Orders.Commands.PlaceOrder;
using DddCqrsJwtDemo.Application.Orders.Queries.GetOrders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DddCqrsJwtDemo.Api.Controllers;

[Authorize]
public class OrdersController : ApiController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await Mediator.Send(new GetOrdersQuery());
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Place([FromBody] PlaceOrderCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(new { OrderId = result });
    }
}
