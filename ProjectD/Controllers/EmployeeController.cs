using Microsoft.AspNetCore.Mvc;
using ProjectD.Models;
using ProjectD.Services;

namespace AzureSqlConnectionDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            var createdEmployee = await _employeeService.CreateEmployeeAsync(employee);
            return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.Id }, createdEmployee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(int id, Employee employee)
        {
            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, employee);
            if (updatedEmployee == null) return NotFound();
            return Ok(updatedEmployee);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftDeleteEmployee(int id)
        {
            var deleted = await _employeeService.SoftDeleteEmployeeAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}