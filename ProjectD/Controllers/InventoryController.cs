using Microsoft.AspNetCore.Mvc;
using AzureSqlConnectionDemo.Models;
using AzureSqlConnectionDemo.Services;

namespace AzureSqlConnectionDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Inventory>>> GetAllInventories()
        {
            var inventories = await _inventoryService.GetAllInventoriesAsync();
            return Ok(inventories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventory(int id)
        {
            var inventory = await _inventoryService.GetInventoryByIdAsync(id);
            if (inventory == null) return NotFound();
            return Ok(inventory);
        }

        [HttpPost]
        public async Task<ActionResult<Inventory>> CreateInventory(Inventory inventory)
        {
            var createdInventory = await _inventoryService.CreateInventoryAsync(inventory);
            return CreatedAtAction(nameof(GetInventory), new { id = createdInventory.Id }, createdInventory);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Inventory>> UpdateInventory(int id, Inventory inventory)
        {
            var updatedInventory = await _inventoryService.UpdateInventoryAsync(id, inventory);
            if (updatedInventory == null) return NotFound();
            return Ok(updatedInventory);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftDeleteInventory(int id)
        {
            var deleted = await _inventoryService.SoftDeleteInventoryAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}