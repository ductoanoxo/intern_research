using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InventoryService.Infrastructure;
using InventoryService.Domain;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("api/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryDbContext _dbContext;

        public InventoryController(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> Get()
        {
            var items = await _dbContext.InventoryItems.ToListAsync();
            return Ok(items);
        }

        [HttpPost("seed")]
        public async Task<IActionResult> Seed()
        {
            if (await _dbContext.InventoryItems.AnyAsync())
            {
                return BadRequest("Inventory already seeded with items.");
            }

            var items = new List<InventoryItem>
            {
                new InventoryItem { ProductId = "laptop", ProductName = "Gaming Laptop", Stock = 10, Price = 1200.00 },
                new InventoryItem { ProductId = "mouse", ProductName = "Wireless Mouse", Stock = 50, Price = 25.50 },
                new InventoryItem { ProductId = "keyboard", ProductName = "Mechanical Keyboard", Stock = 20, Price = 89.99 },
                new InventoryItem { ProductId = "monitor", ProductName = "4K Monitor", Stock = 15, Price = 350.00 }
            };

            await _dbContext.InventoryItems.AddRangeAsync(items);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Inventory seeded successfully", Items = items });
        }
    }
}
