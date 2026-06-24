using System;

namespace MicroservicesDemo.Contracts
{
    public record OrderCreatedEvent
    {
        public Guid OrderId { get; init; }
        public string ProductId { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public double TotalPrice { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
