using Microsoft.AspNetCore.Mvc;
using AzureSqlConnectionDemo.Models;
using AzureSqlConnectionDemo.Services;

namespace AzureSqlConnectionDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Vehicle>>> GetAllVehicles()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vehicle>> GetVehicle(int id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null) return NotFound();
            return Ok(vehicle);
        }

        [HttpPost]
        public async Task<ActionResult<Vehicle>> CreateVehicle(Vehicle vehicle)
        {
            var createdVehicle = await _vehicleService.CreateVehicleAsync(vehicle);
            return CreatedAtAction(nameof(GetVehicle), new { id = createdVehicle.Id }, createdVehicle);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Vehicle>> UpdateVehicle(int id, Vehicle vehicle)
        {
            var updatedVehicle = await _vehicleService.UpdateVehicleAsync(id, vehicle);
            if (updatedVehicle == null) return NotFound();
            return Ok(updatedVehicle);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftDeleteVehicle(int id)
        {
            var deleted = await _vehicleService.SoftDeleteVehicleAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}