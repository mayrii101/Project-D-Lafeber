using Microsoft.EntityFrameworkCore;
using AzureSqlConnectionDemo.Models;

namespace AzureSqlConnectionDemo.Services
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task<Employee> UpdateEmployeeAsync(int id, Employee employee);
        Task<bool> SoftDeleteEmployeeAsync(int id);
    }


    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.Where(e => !e.IsDeleted).ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.Where(e => !e.IsDeleted).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> UpdateEmployeeAsync(int id, Employee employee)
        {
            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null || existingEmployee.IsDeleted) return null;

            existingEmployee.Name = employee.Name;
            existingEmployee.Role = employee.Role;

            await _context.SaveChangesAsync();
            return existingEmployee;
        }

        public async Task<bool> SoftDeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null || employee.IsDeleted) return false;

            employee.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}