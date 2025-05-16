using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureSqlConnectionDemo.Models;
using AzureSqlConnectionDemo.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AzureSqlConnectionDemo.Tests.Services
{
    public class ProductServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new ProductService(_context);

            // Seed data
            _context.Products.AddRange(
                new Product
                {
                    Id = 1,
                    ProductName = "Product A",
                    WeightKg = 10,
                    Material = "Steel",
                    BatchNumber = 100,
                    Price = 49.99,
                    Category = "Tools",
                    IsDeleted = false
                },
                new Product
                {
                    Id = 2,
                    ProductName = "Product B",
                    WeightKg = 5,
                    Material = "Plastic",
                    BatchNumber = 101,
                    Price = 19.99,
                    Category = "Accessories",
                    IsDeleted = true
                }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnOnlyNotDeleted()
        {
            var result = await _service.GetAllProductsAsync();

            Assert.Single(result);
            Assert.Equal("Product A", result.First().ProductName);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnCorrectProduct()
        {
            var result = await _service.GetProductByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Product A", result.ProductName);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNull_IfDeleted()
        {
            var result = await _service.GetProductByIdAsync(2);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldAddProduct()
        {
            var newProduct = new Product
            {
                ProductName = "Product C",
                WeightKg = 8,
                Material = "Wood",
                BatchNumber = 102,
                Price = 29.99,
                Category = "Furniture"
            };

            var result = await _service.CreateProductAsync(newProduct);
            var allProducts = await _context.Products.ToListAsync();

            Assert.Equal(3, allProducts.Count);
            Assert.Equal("Product C", result.ProductName);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateFields()
        {
            var updated = new Product
            {
                ProductName = "Updated Product A",
                WeightKg = 15,
                Material = "Aluminum",
                BatchNumber = 110,
                Price = 59.99,
                Category = "Updated Tools"
            };

            var result = await _service.UpdateProductAsync(1, updated);

            Assert.NotNull(result);
            Assert.Equal("Updated Product A", result.ProductName);
            Assert.Equal("Aluminum", result.Material);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldReturnNull_IfNotFound()
        {
            var updated = new Product { ProductName = "Invalid" };

            var result = await _service.UpdateProductAsync(999, updated);

            Assert.Null(result);
        }

        [Fact]
        public async Task SoftDeleteProductAsync_ShouldMarkAsDeleted()
        {
            var result = await _service.SoftDeleteProductAsync(1);
            var product = await _context.Products.FindAsync(1);

            Assert.True(result);
            Assert.True(product.IsDeleted);
        }

        [Fact]
        public async Task SoftDeleteProductAsync_ShouldReturnFalse_IfAlreadyDeleted()
        {
            var result = await _service.SoftDeleteProductAsync(2);

            Assert.False(result);
        }

        [Fact]
        public async Task SoftDeleteProductAsync_ShouldReturnFalse_IfNotFound()
        {
            var result = await _service.SoftDeleteProductAsync(999);

            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
