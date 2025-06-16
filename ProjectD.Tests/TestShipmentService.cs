using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
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
            Assert.False(_context.Shipments.First().IsDeleted);
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
            var newDto = new ShipmentCreateDto
            {
                VehicleId = 1,
                DriverId = 1,
                Status = ShipmentStatus.OutForDelivery,
                DepartureDate = DateTime.Today.ToString("dd-MM-yyyy"),
                DepartureTime = "10:00:00",
                ExpectedDeliveryDate = DateTime.Today.AddDays(2).ToString("dd-MM-yyyy"),
                ExpectedDeliveryTime = "15:00:00",
                OrderIds = new List<int> { 1 }
            };

            var result = await _service.CreateShipmentAsync(newDto);
            var shipments = await _context.Shipments.ToListAsync();

            Assert.Equal(2, shipments.Count);
            Assert.Equal(ShipmentStatus.OutForDelivery, result.Status);
            Assert.Equal(1, result.OrderIds.Count);
            Assert.Equal(1, result.OrderIds.First());
        }

        [Fact]
        public async Task SoftDeleteShipmentAsync_ShouldSetIsDeleted()
        {
            var result = await _service.SoftDeleteShipmentAsync(1);
            var shipment = await _context.Shipments.FindAsync(1);

            Assert.True(result);
            Assert.True(shipment.IsDeleted);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
