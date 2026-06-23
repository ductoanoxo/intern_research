using System.Collections.Generic;
using DddCqrsJwtDemo.Application.Orders.DTOs;
using MediatR;

namespace DddCqrsJwtDemo.Application.Orders.Queries.GetOrders;

public record GetOrdersQuery : IRequest<List<OrderDto>>;
