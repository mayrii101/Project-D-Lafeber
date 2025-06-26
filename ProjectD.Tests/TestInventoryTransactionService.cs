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
    public class InventoryTransactionServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly InventoryTransactionService _service;

        public InventoryTransactionServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new InventoryTransactionService(_context);

            var product = new Product { Id = 1, ProductName = "Product A" };
            var employee = new Employee { Id = 1, Name = "Employee A" };

            _context.Products.Add(product);
            _context.Employees.Add(employee);
            _context.InventoryTransactions.AddRange(
                new InventoryTransaction
                {
                    Id = 1,
                    ProductId = 1,
                    Quantity = 10,
                    Type = InventoryTransactionType.Inbound,
                    Timestamp = DateTime.UtcNow,
                    SourceOrDestination = "Supplier X",
                    EmployeeId = 1,
                    IsDeleted = false
                },
                new InventoryTransaction
                {
                    Id = 2,
                    ProductId = 1,
                    Quantity = 5,
                    Type = InventoryTransactionType.Outbound,
                    Timestamp = DateTime.UtcNow,
                    SourceOrDestination = "Customer Y",
                    EmployeeId = 1,
                    IsDeleted = true
                }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllInventoryTransactionsAsync_ShouldReturnOnlyNonDeleted()
        {
            var result = await _service.GetAllInventoryTransactionsAsync();

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public async Task GetInventoryTransactionByIdAsync_ShouldReturnCorrectTransaction()
        {
            var result = await _service.GetInventoryTransactionByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(10, result.Quantity);
        }

        [Fact]
        public async Task GetInventoryTransactionByIdAsync_ShouldReturnNullIfDeleted()
        {
            var result = await _service.GetInventoryTransactionByIdAsync(2);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateInventoryTransactionAsync_ShouldAddTransaction()
        {
            var transaction = new InventoryTransaction
            {
                ProductId = 1,
                Quantity = 15,
                Type = InventoryTransactionType.Inbound,
                Timestamp = DateTime.UtcNow,
                SourceOrDestination = "New Supplier",
                EmployeeId = 1
            };

            var result = await _service.CreateInventoryTransactionAsync(transaction);
            var count = await _context.InventoryTransactions.CountAsync();

            Assert.Equal(3, count);
            Assert.Equal(15, result.Quantity);
        }

        [Fact]
        public async Task UpdateInventoryTransactionAsync_ShouldUpdateTransaction()
        {
            var update = new InventoryTransaction
            {
                ProductId = 1,
                Quantity = 99,
                Type = InventoryTransactionType.Outbound,
                Timestamp = DateTime.UtcNow,
                SourceOrDestination = "Updated",
                EmployeeId = 1
            };

            var result = await _service.UpdateInventoryTransactionAsync(1, update);

            Assert.NotNull(result);
            Assert.Equal(99, result.Quantity);
        }

        [Fact]
        public async Task UpdateInventoryTransactionAsync_ShouldReturnNullIfDeleted()
        {
            var update = new InventoryTransaction { ProductId = 1, Quantity = 1, Type = InventoryTransactionType.Outbound, Timestamp = DateTime.UtcNow, SourceOrDestination = "", EmployeeId = 1 };

            var result = await _service.UpdateInventoryTransactionAsync(2, update);

            Assert.Null(result);
        }

        [Fact]
        public async Task SoftDeleteInventoryTransactionAsync_ShouldSetIsDeleted()
        {
            var result = await _service.SoftDeleteInventoryTransactionAsync(1);
            var transaction = await _context.InventoryTransactions.FindAsync(1);

            Assert.True(result);
            Assert.True(transaction.IsDeleted);
        }

        [Fact]
        public async Task SoftDeleteInventoryTransactionAsync_ShouldReturnFalseIfAlreadyDeleted()
        {
            var result = await _service.SoftDeleteInventoryTransactionAsync(2);
            Assert.False(result);
        }

        [Fact]
        public async Task SoftDeleteInventoryTransactionAsync_ShouldReturnFalseIfNotFound()
        {
            var result = await _service.SoftDeleteInventoryTransactionAsync(999);
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
