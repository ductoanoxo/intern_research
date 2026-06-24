using System.Threading.Tasks;
using Grpc.Core;
using InventoryService.Protos;
using InventoryService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryService.Services
{
    public class InventoryGrpcService : InventoryGrpc.InventoryGrpcBase
    {
        private readonly InventoryDbContext _dbContext;
        private readonly ILogger<InventoryGrpcService> _logger;

        public InventoryGrpcService(InventoryDbContext dbContext, ILogger<InventoryGrpcService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public override async Task<CheckStockResponse> CheckStock(CheckStockRequest request, ServerCallContext context)
        {
            _logger.LogInformation("gRPC Request received: Checking stock for Product ID {ProductId}, Quantity requested {Quantity}", request.ProductId, request.Quantity);

            var item = await _dbContext.InventoryItems
                .FirstOrDefaultAsync(i => i.ProductId == request.ProductId);

            if (item == null)
            {
                _logger.LogWarning("Product ID {ProductId} not found in inventory", request.ProductId);
                return new CheckStockResponse
                {
                    IsInStock = false,
                    ProductName = "Unknown Product",
                    Price = 0,
                    CurrentStock = 0
                };
            }

            bool isInStock = item.Stock >= request.Quantity;
            _logger.LogInformation("Product: {ProductName}, Current Stock: {Stock}, Requested: {Quantity}. Result IsInStock: {IsInStock}", 
                item.ProductName, item.Stock, request.Quantity, isInStock);

            return new CheckStockResponse
            {
                IsInStock = isInStock,
                ProductName = item.ProductName,
                Price = item.Price,
                CurrentStock = item.Stock
            };
        }
    }
}
