using System;
using System.Collections.Generic;

namespace DddCqrsJwtDemo.Application.Orders.DTOs;

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    DateTime OrderDate,
    string Status,
    string ShippingAddress,
    List<OrderItemDto> Items,
    decimal TotalPrice,
    string Currency);
