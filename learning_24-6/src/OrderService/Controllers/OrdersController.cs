using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain;
using OrderService.Dtos;
using OrderService.Infrastructure;
using InventoryService.Protos;
using MassTransit;
using MicroservicesDemo.Contracts;
using Microsoft.Extensions.Logging;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _dbContext;
        private readonly InventoryGrpc.InventoryGrpcClient _grpcClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            OrderDbContext dbContext,
            InventoryGrpc.InventoryGrpcClient grpcClient,
            IPublishEndpoint publishEndpoint,
            ILogger<OrdersController> logger)
        {
            _dbContext = dbContext;
            _grpcClient = grpcClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {
            var orders = await _dbContext.Orders.ToListAsync();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            _logger.LogInformation("Creating order for Product: {ProductId}, Quantity: {Quantity}", dto.ProductId, dto.Quantity);

            if (dto.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than 0");
            }

            // 1. Call Inventory Service via gRPC to check stock (Synchronous)
            CheckStockResponse stockResponse;
            try
            {
                stockResponse = await _grpcClient.CheckStockAsync(new CheckStockRequest
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error communicating with Inventory Service via gRPC");
                return StatusCode(503, "Inventory Service is unavailable via gRPC.");
            }

            if (!stockResponse.IsInStock)
            {
                return BadRequest($"Insufficient stock or product '{dto.ProductId}' not found. Current stock available: {stockResponse.CurrentStock}");
            }

            // 2. Create and Save the Order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                ProductId = dto.ProductId,
                ProductName = stockResponse.ProductName,
                Quantity = dto.Quantity,
                Price = stockResponse.Price,
                TotalPrice = stockResponse.Price * dto.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Order {OrderId} successfully saved to DB", order.Id);

            // 3. Publish Order Created Event to RabbitMQ (Asynchronous)
            try
            {
                await _publishEndpoint.Publish(new OrderCreatedEvent
                {
                    OrderId = order.Id,
                    ProductId = order.ProductId,
                    Quantity = order.Quantity,
                    TotalPrice = order.TotalPrice,
                    CreatedAt = order.CreatedAt
                });
                _logger.LogInformation("Successfully published OrderCreatedEvent for Order {OrderId} to RabbitMQ", order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish OrderCreatedEvent to RabbitMQ. Order is saved but inventory decrement might fail.");
            }

            return Ok(new { Message = "Order placed successfully", Order = order });
        }
    }
}
