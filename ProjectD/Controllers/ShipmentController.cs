using Microsoft.AspNetCore.Mvc;
using ProjectD.Models;
using ProjectD.Services;

namespace AzureSqlConnectionDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        public ShipmentController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ShipmentDto>>> GetAllShipments()
        {
            var shipments = await _shipmentService.GetAllShipmentsAsync();
            return Ok(shipments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShipmentDto>> GetShipment(int id)
        {
            var shipment = await _shipmentService.GetShipmentByIdAsync(id);
            if (shipment == null) return NotFound();
            return Ok(shipment);
        }

        [HttpPost]
        public async Task<ActionResult<ShipmentDto>> CreateShipment(ShipmentCreateDto dto)
        {
            try
            {
                var createdShipment = await _shipmentService.CreateShipmentAsync(dto);
                return CreatedAtAction(nameof(GetShipment), new { id = createdShipment.Id }, createdShipment);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(422, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Shipment>> UpdateShipment(int id, Shipment shipment)
        {
            var updatedShipment = await _shipmentService.UpdateShipmentAsync(id, shipment);
            if (updatedShipment == null) return NotFound();
            return Ok(updatedShipment);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftDeleteShipment(int id)
        {
            var deleted = await _shipmentService.SoftDeleteShipmentAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

    }
}