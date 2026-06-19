using System.Collections.Generic;

namespace EcommerceApi.DTOs;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public List<ProductDto> Products { get; set; } = new();
}
