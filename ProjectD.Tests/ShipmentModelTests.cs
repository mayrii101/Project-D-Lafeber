using System;
using System.Collections.Generic;
using System.Linq;
using ProjectD.Models;
using Xunit;

namespace ProjectD.Tests
{
    public class ShipmentModelTests
    {
        [Fact]
        public void OrdersProperty_ShouldReturnOrdersFromShipmentOrders()
        {
            var order1 = new Order { Id = 1, DeliveryAddress = "Teststraat 1" };
            var order2 = new Order { Id = 2, DeliveryAddress = "Teststraat 2" };

            var shipment = new Shipment
            {
                VehicleId = 10,
                DriverId = 5,
                Status = ShipmentStatus.Preparing,
                DepartureDate = DateTime.UtcNow,
                ShipmentOrders = new List<ShipmentOrder>
                {
                    new ShipmentOrder { Order = order1 },
                    new ShipmentOrder { Order = order2 }
                }
            };

            var orders = shipment.Orders;

            Assert.Equal(2, orders.Count);
            Assert.Contains(orders, o => o.Id == order1.Id);
            Assert.Contains(orders, o => o.Id == order2.Id);
        }

        [Fact]
        public void OrdersProperty_ShouldReturnEmptyList_WhenNoShipmentOrders()
        {
            var shipment = new Shipment
            {
                VehicleId = 1,
                DriverId = 2,
                Status = ShipmentStatus.Preparing,
                DepartureDate = DateTime.UtcNow
            };

            var orders = shipment.Orders;

            Assert.NotNull(orders);
            Assert.Empty(orders);
        }

        [Fact]
        public void ExpectedDeliveryDate_ShouldAllowNull()
        {
            var shipment = new Shipment
            {
                VehicleId = 2,
                DriverId = 3,
                DepartureDate = DateTime.UtcNow,
                Status = ShipmentStatus.OutForDelivery,
                ExpectedDeliveryDate = null
            };

            Assert.Null(shipment.ExpectedDeliveryDate);
        }

        [Fact]
        public void ActualDeliveryDate_ShouldAllowNull()
        {
            var shipment = new Shipment
            {
                VehicleId = 2,
                DriverId = 3,
                DepartureDate = DateTime.UtcNow,
                Status = ShipmentStatus.Delivered,
                ActualDeliveryDate = null
            };

            Assert.Null(shipment.ActualDeliveryDate);
        }

        [Fact]
        public void IsDeleted_ShouldBeFalseByDefault()
        {
            var shipment = new Shipment
            {
                VehicleId = 1,
                DriverId = 1,
                Status = ShipmentStatus.Preparing,
                DepartureDate = DateTime.UtcNow
            };

            Assert.False(shipment.IsDeleted);
        }

        [Theory]
        [InlineData(ShipmentStatus.Preparing)]
        [InlineData(ShipmentStatus.OutForDelivery)]
        [InlineData(ShipmentStatus.Delivered)]
        public void Shipment_ShouldAcceptAllValidStatuses(ShipmentStatus status)
        {
            var shipment = new Shipment
            {
                VehicleId = 1,
                DriverId = 1,
                Status = status,
                DepartureDate = DateTime.UtcNow
            };

            Assert.Equal(status, shipment.Status);
        }
    }
}
