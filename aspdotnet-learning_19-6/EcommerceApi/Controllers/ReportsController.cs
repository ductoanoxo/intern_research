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
public class ReportsController : ControllerBase
{
    private readonly EcommerceContext _context;

    public ReportsController(EcommerceContext context)
    {
        _context = context;
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        // 1. Calculate general stats
        double totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
        int totalOrders = await _context.Orders.CountAsync();

        // 2. Monthly Revenue - Group by month
        var orders = await _context.Orders.Select(o => new { o.OrderDate, o.TotalAmount }).ToListAsync();
        var monthlyRevenue = orders
            .GroupBy(o => o.OrderDate.HasValue ? o.OrderDate.Value.ToString("yyyy-MM") : "Unknown")
            .Select(g => new MonthlyRevenueDto
            {
                Month = g.Key,
                Revenue = g.Sum(o => o.TotalAmount)
            })
            .OrderBy(mr => mr.Month)
            .ToList();

        // 3. Top selling products using GroupBy and Join in LINQ
        var topProductsQuery = _context.OrderDetails
            .GroupBy(od => od.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalQuantitySold = g.Sum(od => od.Quantity),
                TotalRevenue = g.Sum(od => od.Quantity * od.UnitPrice)
            })
            .OrderByDescending(tp => tp.TotalQuantitySold)
            .Take(5);

        var topProducts = await topProductsQuery
            .Join(_context.Products, 
                tp => tp.ProductId, 
                p => p.ProductId, 
                (tp, p) => new TopProductDto
                {
                    ProductId = tp.ProductId,
                    ProductName = p.Name,
                    TotalQuantitySold = tp.TotalQuantitySold,
                    TotalRevenue = tp.TotalRevenue
                })
            .ToListAsync();

        var stats = new SalesStatisticsDto
        {
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            MonthlyRevenue = monthlyRevenue,
            TopSellingProducts = topProducts
        };

        return Ok(stats);
    }
}
