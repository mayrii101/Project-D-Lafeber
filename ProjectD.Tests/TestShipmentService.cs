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
    public class ShipmentServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ShipmentService _service;

        public ShipmentServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            SeedData();
            _service = new ShipmentService(_context);
        }
        // 123
        private void SeedData()
        {
            var vehicle = new Vehicle { Id = 1, Type = VehicleType.FlatbedTrailer, Status = VehicleStatus.Available };
            var driver = new Employee { Id = 1, Name = "Jan" };

            var order = new Order
            {
                Id = 1,
                CustomerId = 1,
                OrderDate = DateTime.Now,
                DeliveryAddress = "Teststraat 1",
                ExpectedDeliveryDate = DateTime.Now.AddDays(2),
                Status = OrderStatus.Pending,
                ProductLines = new List<OrderLine>(),
                ShipmentOrders = new List<ShipmentOrder>()
            };

            var shipment = new Shipment
            {
                Id = 1,
                Vehicle = vehicle,
                Driver = driver,
                VehicleId = 1,
                DriverId = 1,
                Status = ShipmentStatus.Preparing,
                DepartureDate = DateTime.Now,
                ShipmentOrders = new List<ShipmentOrder>(),
                IsDeleted = false
            };

            _context.Vehicles.Add(vehicle);
            _context.Employees.Add(driver);
            _context.Orders.Add(order);
            _context.Shipments.Add(shipment);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllShipmentsAsync_ShouldReturnNonDeletedOnly()
        {
            var shipments = await _service.GetAllShipmentsAsync();

            Assert.Single(shipments);
            Assert.False(shipments[0].IsDeleted);
        }

        [Fact]
        public async Task GetShipmentByIdAsync_ShouldReturnShipment_WhenExists()
        {
            var shipment = await _service.GetShipmentByIdAsync(1);

            Assert.NotNull(shipment);
            Assert.Equal(ShipmentStatus.Preparing, shipment.Status);
        }

        [Fact]
        public async Task CreateShipmentAsync_ShouldAddShipment()
        {
            var newShipment = new Shipment
            {
                VehicleId = 1,
                DriverId = 1,
                Status = ShipmentStatus.OutForDelivery,
                DepartureDate = DateTime.Now
            };

            var result = await _service.CreateShipmentAsync(newShipment);
            var shipments = await _context.Shipments.ToListAsync();

            Assert.Equal(2, shipments.Count);
            Assert.Equal(ShipmentStatus.OutForDelivery, result.Status);
        }

        // [Fact]
        // public async Task UpdateShipmentAsync_ShouldModifyShipmentAndAddOrders()
        // {
        //     var order2 = new Order
        //     {
        //         Id = 2,
        //         CustomerId = 1,
        //         OrderDate = DateTime.Now,
        //         DeliveryAddress = "Nieuweweg 2",
        //         ExpectedDeliveryDate = DateTime.Now.AddDays(3),
        //         Status = OrderStatus.Processing,
        //         ProductLines = new List<OrderLine>(),
        //         ShipmentOrders = new List<ShipmentOrder>()
        //     };

        //     _context.Orders.Add(order2);
        //     _context.SaveChanges();

        //     var update = new Shipment
        //     {
        //         Vehicle = _context.Vehicles.Find(1),
        //         Driver = _context.Employees.Find(1),
        //         Status = ShipmentStatus.Delivered,
        //         DepartureDate = DateTime.Now,
        //         Orders = new List<Order> { order2 }
        //     };

        //     var updated = await _service.UpdateShipmentAsync(1, update);

        //     Assert.NotNull(updated);
        //     Assert.Equal(ShipmentStatus.Delivered, updated.Status);
        //     Assert.Contains(updated.Orders, o => o.Id == 2);
        // }

        [Fact]
        public async Task SoftDeleteShipmentAsync_ShouldSetIsDeleted()
        {
            var result = await _service.SoftDeleteShipmentAsync(1);
            var shipment = await _context.Shipments.FindAsync(1);

            Assert.True(result);
            Assert.True(shipment.IsDeleted);
        }

        [Fact]
        public void CreateShipmentOrder_ShouldLinkOrderAndShipment()
        {
            var result = _service.CreateShipmentOrder(1, 1);
            var shipmentOrder = _context.ShipmentOrders.FirstOrDefault();

            Assert.True(result);
            Assert.NotNull(shipmentOrder);
            Assert.Equal(1, shipmentOrder.OrderId);
            Assert.Equal(1, shipmentOrder.ShipmentId);
        }

        [Fact]
        public void CreateShipmentOrder_ShouldReturnFalse_WhenInvalid()
        {
            var result = _service.CreateShipmentOrder(999, 999);
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
