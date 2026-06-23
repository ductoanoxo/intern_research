using System;
using System.Collections.Generic;
using MediatR;

namespace DddCqrsJwtDemo.Application.Orders.Commands.PlaceOrder;

public record OrderItemCommand(Guid BookId, int Quantity);

public record PlaceOrderCommand(
    string Street,
    string City,
    string ZipCode,
    List<OrderItemCommand> Items) : IRequest<Guid>;
