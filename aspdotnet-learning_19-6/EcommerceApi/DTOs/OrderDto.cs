using System;
using System.Collections.Generic;

namespace EcommerceApi.DTOs;

public class OrderDto
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;
    public DateTime? OrderDate { get; set; }
    public double TotalAmount { get; set; }
    public string Status { get; set; } = null!;
    public List<OrderDetailDto> OrderDetails { get; set; } = new();
}

public class OrderDetailDto
{
    public int OrderDetailId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double SubTotal => Quantity * UnitPrice;
}

public class OrderCreateDto
{
    public int CustomerId { get; set; }
    public List<OrderItemCreateDto> Items { get; set; } = new();
}

public class OrderItemCreateDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
