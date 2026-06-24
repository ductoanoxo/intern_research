namespace InventoryService.Domain
{
    public class InventoryItem
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Stock { get; set; }
        public double Price { get; set; }
    }
}
