using System;
using System.Threading.Tasks;
using AzureSqlConnectionDemo.Models;
using AzureSqlConnectionDemo.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AzureSqlConnectionDemo.Tests.Services
{
    public class InventoryServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly InventoryService _service;

        public InventoryServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new InventoryService(_context);

            _context.Inventories.AddRange(
                new Inventory { Id = 1, ProductId = 101, WarehouseId = 201, QuantityOnHand = 50, IsDeleted = false },
                new Inventory { Id = 2, ProductId = 102, WarehouseId = 202, QuantityOnHand = 100, IsDeleted = true }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllInventoriesAsync_ShouldReturnOnlyNonDeleted()
        {
            var result = await _service.GetAllInventoriesAsync();

            Assert.Single(result);
            Assert.Equal(101, result[0].ProductId);
        }

        [Fact]
        public async Task GetInventoryByIdAsync_ShouldReturnCorrectInventory()
        {
            var result = await _service.GetInventoryByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(201, result.WarehouseId);
        }

        [Fact]
        public async Task CreateInventoryAsync_ShouldAddInventory()
        {
            var newInventory = new Inventory
            {
                ProductId = 103,
                WarehouseId = 203,
                QuantityOnHand = 75,
                LastUpdated = DateTime.Now
            };

            var result = await _service.CreateInventoryAsync(newInventory);
            var inventories = await _context.Inventories.ToListAsync();

            Assert.Equal(3, inventories.Count);
            Assert.Equal(103, result.ProductId);
        }

        [Fact]
        public async Task UpdateInventoryAsync_ShouldUpdateInventory()
        {
            var update = new Inventory
            {
                ProductId = 111,
                WarehouseId = 211,
                QuantityOnHand = 60
            };

            var result = await _service.UpdateInventoryAsync(1, update);

            Assert.NotNull(result);
            Assert.Equal(111, result.ProductId);
            Assert.Equal(60, result.QuantityOnHand);
        }

        [Fact]
        public async Task UpdateInventoryAsync_ShouldReturnNullIfNotFound()
        {
            var update = new Inventory { ProductId = 999 };

            var result = await _service.UpdateInventoryAsync(999, update);

            Assert.Null(result);
        }

        [Fact]
        public async Task SoftDeleteInventoryAsync_ShouldSetIsDeleted()
        {
            var result = await _service.SoftDeleteInventoryAsync(1);
            var inventory = await _context.Inventories.FindAsync(1);

            Assert.True(result);
            Assert.True(inventory.IsDeleted);
        }

        [Fact]
        public async Task SoftDeleteInventoryAsync_ShouldReturnFalseIfAlreadyDeleted()
        {
            var result = await _service.SoftDeleteInventoryAsync(2);

            Assert.False(result);
        }

        [Fact]
        public async Task SoftDeleteInventoryAsync_ShouldReturnFalseIfNotFound()
        {
            var result = await _service.SoftDeleteInventoryAsync(999);

            Assert.False(result);
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}