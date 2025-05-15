using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AzureSqlConnectionDemo.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if there is any data already in the database
            if (context.Customers.Any() || context.Products.Any() || context.Warehouses.Any() || context.Employees.Any() || context.Orders.Any())
                return; // Data already exists, no need to seed

            // Add Customers
            var customer1 = new Customer
            {
                BedrijfsNaam = "Bouwkracht BV",
                ContactPersoon = "Kees de Bruin",
                Email = "k.bruin@bouwkrachtbv.nl",
                TelefoonNummer = "+31-409678581",
                Adres = "Industrieweg 12, Utrecht",
                IsDeleted = false
            };

            var customer2 = new Customer
            {
                BedrijfsNaam = "InfraTech Solutions",
                ContactPersoon = "Sanne Willems",
                Email = "sanne.w@infratechsol.com",
                TelefoonNummer = "+31-809258582",
                Adres = "Deltaweg 88, Zeeland",
                IsDeleted = false
            };

            context.Customers.AddRange(customer1, customer2);

            // Add Products
            var product1 = new Product
            {
                ProductName = "Gewapend beton",
                WeightKg = 6500,
                Material = "Metal",
                BatchNumber = 1001,
                Price = 3800,
                Category = "Ruwbouw",
                IsDeleted = false
            };

            var product2 = new Product
            {
                ProductName = "Stalen I-balk (HEB 500)",
                WeightKg = 1210,
                Material = "Plastic",
                BatchNumber = 1002,
                Price = 1950,
                Category = "Constructiestaal",
                IsDeleted = false
            };

            context.Products.AddRange(product1, product2);

            // Add Warehouses
            var warehouse1 = new Warehouse
            {
                Name = "Lafeber Heavy Logistics Hub",
                Location = "Gouda, Netherlands,",
                ContactPerson = "Alice van Dijk",
                Phone = "+31-101234567",
                IsDeleted = false
            };

            var warehouse2 = new Warehouse
            {
                Name = "Lafeber Inland Storage Facility",
                Location = "Eindhoven, Netherlands",
                ContactPerson = "Bob Meijer",
                Phone = "+31-409876543",
                IsDeleted = false
            };

            context.Warehouses.AddRange(warehouse1, warehouse2);

            // Call SaveChanges to generate the IDs for the Customers, Products, and Warehouses
            context.SaveChanges();

            // Add Inventory
            context.Inventories.AddRange(
                new Inventory
                {
                    ProductId = product1.Id,
                    WarehouseId = warehouse1.Id,
                    QuantityOnHand = 100,
                    LastUpdated = DateTime.Now,
                    IsDeleted = false
                },
                new Inventory
                {
                    ProductId = product2.Id,
                    WarehouseId = warehouse2.Id,
                    QuantityOnHand = 50,
                    LastUpdated = DateTime.Now,
                    IsDeleted = false
                }
            );

            // Add Employees
            var employee1 = new Employee
            {
                Name = "Mark Jansen",
                Role = "Driver",
                Email = "mark.jansen@lafeberlogistics.nl",
                IsDeleted = false
            };

            var employee2 = new Employee
            {
                Name = "Fatima El Idrissi",
                Role = "Driver",
                Email = "fatima.elidrissi@lafeberlogistics.nl",
                IsDeleted = false
            };

            context.Employees.AddRange(employee1, employee2);

            // Add Orders
            var order1 = new Order
            {
                CustomerId = customer1.Id,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.Now,
                DeliveryAddress = "Deltaweg 88, Zeeland",
                ExpectedDeliveryDate = DateTime.Now.AddDays(5),
                ActualDeliveryDate = null,
                IsDeleted = false
            };

            var order2 = new Order
            {
                CustomerId = customer2.Id,
                Status = OrderStatus.Processing,
                OrderDate = DateTime.Now,
                DeliveryAddress = "Industrieweg 12, Utrecht",
                ExpectedDeliveryDate = DateTime.Now.AddDays(3),
                ActualDeliveryDate = null,
                IsDeleted = false
            };

            context.Orders.AddRange(order1, order2);

            // Call SaveChanges to generate the IDs for Orders
            context.SaveChanges();

            // Add OrderLines
            context.OrderLines.AddRange(
                new OrderLine
                {
                    OrderId = order1.Id,
                    ProductId = product1.Id,
                    Quantity = 2,
                    IsDeleted = false
                },
                new OrderLine
                {
                    OrderId = order2.Id,
                    ProductId = product2.Id,
                    Quantity = 1,
                    IsDeleted = false
                }
            );

            // Add Vehicles
            var vehicle1 = new Vehicle
            {
                LicensePlate = "ABC123",
                CapacityKg = 25000,
                Type = VehicleType.FlatbedTrailer,
                Status = VehicleStatus.Available,
                IsDeleted = false
            };

            var vehicle2 = new Vehicle
            {
                LicensePlate = "XYZ456",
                CapacityKg = 60000,
                Type = VehicleType.LowbedTrailer,
                Status = VehicleStatus.InUse,
                IsDeleted = false
            };

            context.Vehicles.AddRange(vehicle1, vehicle2);

            // Call SaveChanges to generate the IDs for Vehicles
            context.SaveChanges();

            // Add Shipments
            var shipment1 = new Shipment
            {
                VehicleId = vehicle1.Id,
                DriverId = employee1.Id,
                Status = ShipmentStatus.Preparing,
                DepartureDate = DateTime.Now
            };

            var shipment2 = new Shipment
            {
                VehicleId = vehicle2.Id,
                DriverId = employee2.Id,
                Status = ShipmentStatus.OutForDelivery,
                DepartureDate = DateTime.Now
            };

            context.Shipments.AddRange(shipment1, shipment2);

            // Save all the changes to the database
            context.SaveChanges();

            // Add ShipmentOrders to link Shipments to Orders
            var shipmentOrder1 = new ShipmentOrder
            {
                ShipmentId = shipment1.Id,
                OrderId = order1.Id
            };

            var shipmentOrder2 = new ShipmentOrder
            {
                ShipmentId = shipment2.Id,
                OrderId = order2.Id
            };

            context.ShipmentOrders.AddRange(shipmentOrder1, shipmentOrder2);

            // Final SaveChanges to persist ShipmentOrders in the database
            context.SaveChanges();
        }
    }
}



