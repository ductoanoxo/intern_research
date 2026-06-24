using System.Threading.Tasks;
using MassTransit;
using MicroservicesDemo.Contracts;
using InventoryService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryService.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly InventoryDbContext _dbContext;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(InventoryDbContext dbContext, ILogger<OrderCreatedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("RabbitMQ Event Consumed: OrderCreatedEvent for Order {OrderId}, Product: {ProductId}, Qty: {Quantity}",
                message.OrderId, message.ProductId, message.Quantity);

            var item = await _dbContext.InventoryItems
                .FirstOrDefaultAsync(i => i.ProductId == message.ProductId);

            if (item != null)
            {
                int oldStock = item.Stock;
                item.Stock -= message.Quantity;
                if (item.Stock < 0) item.Stock = 0;

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Inventory updated for Product {ProductId}: Stock decremented from {OldStock} to {NewStock}",
                    message.ProductId, oldStock, item.Stock);
            }
            else
            {
                _logger.LogError("Inventory item not found for Product ID {ProductId} during OrderCreatedEvent consumption!", message.ProductId);
            }
        }
    }
}
