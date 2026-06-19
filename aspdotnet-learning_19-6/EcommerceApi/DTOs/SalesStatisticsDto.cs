using System.Collections.Generic;

namespace EcommerceApi.DTOs;

public class SalesStatisticsDto
{
    public double TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
    public List<TopProductDto> TopSellingProducts { get; set; } = new();
}

public class MonthlyRevenueDto
{
    public string Month { get; set; } = null!;
    public double Revenue { get; set; }
}

public class TopProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int TotalQuantitySold { get; set; }
    public double TotalRevenue { get; set; }
}
