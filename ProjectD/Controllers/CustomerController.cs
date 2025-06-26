using Microsoft.AspNetCore.Mvc;
using ProjectD.Models;
using ProjectD.Services;

namespace AzureSqlConnectionDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            var createdCustomer = await _customerService.CreateCustomerAsync(customer);
            return CreatedAtAction(nameof(GetCustomer), new { id = createdCustomer.Id }, createdCustomer);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Customer>> UpdateCustomer(int id, Customer customer)
        {
            var updatedCustomer = await _customerService.UpdateCustomerAsync(id, customer);
            if (updatedCustomer == null) return NotFound();
            return Ok(updatedCustomer);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftDeleteCustomer(int id)
        {
            var deleted = await _customerService.SoftDeleteCustomerAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}