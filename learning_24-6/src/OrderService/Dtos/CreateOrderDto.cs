namespace OrderService.Dtos
{
    public class CreateOrderDto
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
