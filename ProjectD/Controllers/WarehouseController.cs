using Microsoft.AspNetCore.Mvc;
using ProjectD.Models;
using ProjectD.Services;

namespace AzureSqlConnectionDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Warehouse>>> GetAllWarehouses()
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            return Ok(warehouses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouse(int id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null) return NotFound();
            return Ok(warehouse);
        }

        [HttpPost]
        public async Task<ActionResult<Warehouse>> CreateWarehouse(Warehouse warehouse)
        {
            var createdWarehouse = await _warehouseService.CreateWarehouseAsync(warehouse);
            return CreatedAtAction(nameof(GetWarehouse), new { id = createdWarehouse.Id }, createdWarehouse);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Warehouse>> UpdateWarehouse(int id, Warehouse warehouse)
        {
            var updatedWarehouse = await _warehouseService.UpdateWarehouseAsync(id, warehouse);
            if (updatedWarehouse == null) return NotFound();
            return Ok(updatedWarehouse);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftDeleteWarehouse(int id)
        {
            var deleted = await _warehouseService.SoftDeleteWarehouseAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}