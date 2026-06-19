using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceApi.Models;
using EcommerceApi.DTOs;

namespace EcommerceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly EcommerceContext _context;

    public ProductsController(EcommerceContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] double? minPrice,
        [FromQuery] double? maxPrice,
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDesc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // 1. Start with IQueryable for Deferred Execution
        IQueryable<Product> query = _context.Products.Include(p => p.Category);

        // 2. Apply filters (Deferred Execution)
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            string searchLower = search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchLower) 
                                     || (p.Description != null && p.Description.ToLower().Contains(searchLower)));
        }

        // 3. Apply sorting (Deferred Execution)
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "price" => isDesc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "name" => isDesc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "createdat" => isDesc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                _ => query.OrderBy(p => p.ProductId) // Default sort
            };
        }
        else
        {
            query = query.OrderBy(p => p.ProductId);
        }

        // 4. Apply pagination & Immediate Execution (using ToListAsync)
        int totalItems = await query.CountAsync();
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Description = p.Description,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
            Data = products
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
            return NotFound($"Product with ID {id} not found.");

        var dto = new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Description = product.Description,
            CreatedAt = product.CreatedAt
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
        if (!categoryExists)
            return BadRequest($"Category with ID {dto.CategoryId} does not exist.");

        var product = new Product
        {
            Name = dto.Name,
            CategoryId = dto.CategoryId,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            CategoryId = product.CategoryId,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Description = product.Description,
            CreatedAt = product.CreatedAt
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductCreateDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound($"Product with ID {id} not found.");

        var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
        if (!categoryExists)
            return BadRequest($"Category with ID {dto.CategoryId} does not exist.");

        product.Name = dto.Name;
        product.CategoryId = dto.CategoryId;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.Description = dto.Description;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound($"Product with ID {id} not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
