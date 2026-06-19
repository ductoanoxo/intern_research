using System;

namespace EcommerceApi.DTOs;

public class ProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public double Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class ProductCreateDto
{
    public string Name { get; set; } = null!;
    public int CategoryId { get; set; }
    public double Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Description { get; set; }
}
