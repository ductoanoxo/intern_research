using System;

namespace DddCqrsJwtDemo.Application.Orders.DTOs;

public record OrderItemDto(
    Guid BookId,
    string BookTitle,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);
