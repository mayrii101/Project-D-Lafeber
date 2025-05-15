using AzureSqlConnectionDemo.Models;
using AzureSqlConnectionDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureSqlConnectionDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryTransactionController : ControllerBase
    {
        private readonly IInventoryTransactionService _inventoryTransactionService;

        // Inject the service into the controller
        public InventoryTransactionController(IInventoryTransactionService inventoryTransactionService)
        {
            _inventoryTransactionService = inventoryTransactionService;
        }

        // GET: api/InventoryTransaction
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryTransaction>>> GetInventoryTransactions()
        {
            var transactions = await _inventoryTransactionService.GetAllInventoryTransactionsAsync();
            return Ok(transactions); // Returns the list of inventory transactions
        }

        // GET: api/InventoryTransaction/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryTransaction>> GetInventoryTransactionById(int id)
        {
            var transaction = await _inventoryTransactionService.GetInventoryTransactionByIdAsync(id);

            if (transaction == null)
            {
                return NotFound(); // Return 404 if not found
            }

            return Ok(transaction); // Return the found transaction
        }

        // POST: api/InventoryTransaction
        [HttpPost]
        public async Task<ActionResult<InventoryTransaction>> CreateInventoryTransaction(InventoryTransaction transaction)
        {
            var createdTransaction = await _inventoryTransactionService.CreateInventoryTransactionAsync(transaction);

            return CreatedAtAction(nameof(GetInventoryTransactionById), new { id = createdTransaction.Id }, createdTransaction);
            // Return 201 Created with a location header
        }

        // PUT: api/InventoryTransaction/5
        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryTransaction>> UpdateInventoryTransaction(int id, InventoryTransaction transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest(); // If IDs don't match, return 400 Bad Request
            }

            var updatedTransaction = await _inventoryTransactionService.UpdateInventoryTransactionAsync(id, transaction);

            if (updatedTransaction == null)
            {
                return NotFound(); // Return 404 if transaction wasn't found or updated
            }

            return Ok(updatedTransaction); // Return the updated transaction
        }

        // DELETE: api/InventoryTransaction/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteInventoryTransaction(int id)
        {
            var success = await _inventoryTransactionService.SoftDeleteInventoryTransactionAsync(id);

            if (!success)
            {
                return NotFound(); // Return 404 if transaction wasn't found or already deleted
            }

            return NoContent(); // Return 204 No Content for successful soft delete
        }
    }
}