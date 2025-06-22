using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
using System.Globalization;
using Xunit;

namespace AzureSqlConnectionDemo.Tests.Services
{
    public class OrderServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new OrderService(_context);

            // Seed customer and product
            var customer = new Customer { Id = 1, BedrijfsNaam = "Test Customer", IsDeleted = false };
            var product = new Product { Id = 1, ProductName = "Test Product", WeightKg = 10 };
            _context.Customers.Add(customer);
            _context.Products.Add(product);

            // Seed inventory with enough stock for ProductId = 1
            var inventory = new Inventory
            {
                Id = 1,
                ProductId = 1,
                WarehouseId = 1,
                QuantityOnHand = 10,   // Sufficient stock
                LastUpdated = DateTime.UtcNow,
                IsDeleted = false
            };
            _context.Inventories.Add(inventory);

            // Seed existing orders
            var orderLine = new OrderLine { ProductId = 1, Product = product, Quantity = 2 };

            _context.Orders.AddRange(
                new Order
                {
                    Id = 1,
                    CustomerId = 1,
                    Customer = customer,
                    OrderDate = DateTime.Today,
                    DeliveryAddress = "Street 1",
                    ExpectedDeliveryDate = DateTime.Today.AddDays(5),
                    Status = OrderStatus.Pending,
                    ProductLines = new List<OrderLine> { orderLine },
                    IsDeleted = false
                },
                new Order
                {
                    Id = 2,
                    CustomerId = 1,
                    Customer = customer,
                    OrderDate = DateTime.Today,
                    DeliveryAddress = "Street 2",
                    ExpectedDeliveryDate = DateTime.Today.AddDays(5),
                    Status = OrderStatus.Pending,
                    ProductLines = new List<OrderLine> { orderLine },
                    IsDeleted = true
                }
            );

            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnOnlyNonDeleted()
        {
            var result = await _service.GetAllOrdersAsync();

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrder_IfNotDeleted()
        {
            var result = await _service.GetOrderByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Street 1", result.DeliveryAddress);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnNull_IfDeleted()
        {
            var result = await _service.GetOrderByIdAsync(2);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldAddOrder_WhenStockIsSufficient()
        {
            // Arrange
            var newOrderDto = new OrderCreateDto
            {
                CustomerId = 1,
                OrderDate = DateTime.Today.ToString("dd-MM-yyyy"),
                OrderTime = "10:00:00",
                DeliveryAddress = "New Address",
                ExpectedDeliveryDate = DateTime.Today.AddDays(3).ToString("dd-MM-yyyy"),
                ExpectedDeliveryTime = "14:00:00",
                Status = OrderStatus.Processing,
                ProductLines = new List<OrderLineCreateDto>
        {
            new OrderLineCreateDto { ProductId = 1, Quantity = 2 }
        }
            };


            var result = await _service.CreateOrderAsync(newOrderDto);
            var orders = await _context.Orders.Include(o => o.ProductLines).ToListAsync();

            var parsedOrderDate = DateTime.ParseExact(result.OrderDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var parsedExpectedDate = DateTime.ParseExact(result.ExpectedDeliveryDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            Assert.Equal(3, orders.Count);
            Assert.Equal("New Address", result.DeliveryAddress);
            Assert.Single(result.ProductLines);
            Assert.Equal(1, result.ProductLines.First().ProductId);
            Assert.Equal(2, result.ProductLines.First().Quantity);
            Assert.Equal(DateTime.Today, parsedOrderDate.Date);
            Assert.Equal(DateTime.Today.AddDays(3), parsedExpectedDate.Date);
            Assert.Equal("Bestelling geplaatst!.", result.Message);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldUpdateOrder()
        {
            var updatedOrder = new Order
            {
                CustomerId = 1,
                OrderDate = DateTime.Today.AddDays(1),
                DeliveryAddress = "Updated Address",
                ExpectedDeliveryDate = DateTime.Today.AddDays(4),
                Status = OrderStatus.Shipped,
                ProductLines = new List<OrderLine>
                {
                    new OrderLine { ProductId = 1, Quantity = 3 }
                }
            };

            var result = await _service.UpdateOrderAsync(1, updatedOrder);

            Assert.NotNull(result);
            Assert.Equal("Updated Address", result.DeliveryAddress);
            Assert.Equal(OrderStatus.Shipped, result.Status);
            Assert.Single(result.ProductLines);
            Assert.Equal(3, result.ProductLines.First().Quantity);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldReturnNull_IfNotFound()
        {
            var update = new Order { CustomerId = 1 };
            var result = await _service.UpdateOrderAsync(999, update);

            Assert.Null(result);
        }

        [Fact]
        public async Task SoftDeleteOrderAsync_ShouldMarkOrderAsDeleted()
        {
            var result = await _service.SoftDeleteOrderAsync(1);
            var order = await _context.Orders.FindAsync(1);

            Assert.True(result);
            Assert.True(order.IsDeleted);
        }

        [Fact]
        public async Task SoftDeleteOrderAsync_ShouldReturnFalse_IfAlreadyDeleted()
        {
            var result = await _service.SoftDeleteOrderAsync(2);

            Assert.False(result);
        }

        [Fact]
        public async Task SoftDeleteOrderAsync_ShouldReturnFalse_IfNotFound()
        {
            var result = await _service.SoftDeleteOrderAsync(999);

            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }


}
