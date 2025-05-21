using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectD.Models;
using ProjectD.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AzureSqlConnectionDemo.Tests.Services
{
    public class EmployeeServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new EmployeeService(_context);

            _context.Employees.AddRange(
                new Employee { Id = 1, Name = "John Doe", Role = "Developer", IsDeleted = false },
                new Employee { Id = 2, Name = "Jane Smith", Role = "Manager", IsDeleted = true }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllEmployeesAsync_ShouldReturnOnlyNonDeleted()
        {
            var result = await _service.GetAllEmployeesAsync();

            Assert.Single(result);
            Assert.Equal("John Doe", result.First().Name);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ShouldReturnCorrectEmployee()
        {
            var result = await _service.GetEmployeeByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Developer", result.Role);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ShouldReturnNullIfDeleted()
        {
            var result = await _service.GetEmployeeByIdAsync(2);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ShouldReturnNullIfNotFound()
        {
            var result = await _service.GetEmployeeByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateEmployeeAsync_ShouldAddEmployee()
        {
            var newEmployee = new Employee
            {
                Name = "New Employee",
                Role = "Tester"
            };

            var result = await _service.CreateEmployeeAsync(newEmployee);
            var employees = await _context.Employees.ToListAsync();

            Assert.Equal(3, employees.Count);
            Assert.Equal("Tester", result.Role);
            Assert.False(result.IsDeleted);
        }

        [Fact]
        public async Task CreateEmployeeAsync_ShouldThrowForNullInput()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.CreateEmployeeAsync(null));
        }

        [Fact]
        public async Task UpdateEmployeeAsync_ShouldUpdateEmployee()
        {
            var update = new Employee
            {
                Name = "Updated Name",
                Role = "Senior Developer"
            };

            var result = await _service.UpdateEmployeeAsync(1, update);

            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            Assert.Equal("Senior Developer", result.Role);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_ShouldReturnNullIfNotFound()
        {
            var update = new Employee { Name = "Non-existent" };

            var result = await _service.UpdateEmployeeAsync(999, update);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_ShouldReturnNullIfDeleted()
        {
            var update = new Employee { Name = "Should Fail" };

            var result = await _service.UpdateEmployeeAsync(2, update);

            Assert.Null(result);
        }

        [Fact]
        public async Task SoftDeleteEmployeeAsync_ShouldSetIsDeleted()
        {
            var result = await _service.SoftDeleteEmployeeAsync(1);
            var employee = await _context.Employees.FindAsync(1);

            Assert.True(result);
            Assert.True(employee.IsDeleted);
        }

        [Fact]
        public async Task SoftDeleteEmployeeAsync_ShouldReturnFalseIfAlreadyDeleted()
        {
            var result = await _service.SoftDeleteEmployeeAsync(2);

            Assert.False(result);
        }

        [Fact]
        public async Task SoftDeleteEmployeeAsync_ShouldReturnFalseIfNotFound()
        {
            var result = await _service.SoftDeleteEmployeeAsync(999);

            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}