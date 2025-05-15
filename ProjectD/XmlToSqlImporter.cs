using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AzureSqlConnectionDemo.Models;
using EFCore.BulkExtensions;

public class XmlToSqlImporter
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<XmlToSqlImporter> _logger;

    public XmlToSqlImporter(ApplicationDbContext context, ILogger<XmlToSqlImporter> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ImportAllAsync(string xmlPath)
    {
        try
        {
            var xml = XDocument.Load(xmlPath);
            
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                await ImportCustomersAsync(xml);
                await ImportEmployeesAsync(xml);
                await ImportWarehousesAsync(xml);
                await ImportProductsAsync(xml);
                await ImportVehiclesAsync(xml);
                await ImportOrdersAsync(xml);
                await ImportOrderLinesAsync(xml);
                await ImportShipmentsAsync(xml);
                await ImportShipmentOrdersAsync(xml);
                await ImportInventoryAsync(xml);
                await ImportInventoryTransactionsAsync(xml);

                await transaction.CommitAsync();
                _logger.LogInformation("All data imported successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Transaction rolled back");
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "XML import failed");
            throw;
        }
    }

    // ===== INDIVIDUAL IMPORT METHODS =====
    private async Task ImportCustomersAsync(XDocument xml)
    {
        var entities = xml.Descendants("Customer")
            .Select(x => new Customer
            {
                Id = SafeGetInt(x, "Id"),
                BedrijfsNaam = SafeGetString(x, "BedrijfsNaam"),
                ContactPersoon = SafeGetString(x, "ContactPersoon"),
                Email = SafeGetString(x, "Email"),
                TelefoonNummer = SafeGetString(x, "TelefoonNummer"),
                Adres = SafeGetString(x, "Adres"),
                IsDeleted = SafeGetBool(x, "IsDeleted")
            }).ToList();

        await _context.BulkInsertOrUpdateAsync(entities, new BulkConfig
        {
            UpdateByProperties = new List<string> { nameof(Customer.Id) }
        });
    }

    private async Task ImportEmployeesAsync(XDocument xml)
    {
        var entities = xml.Descendants("Employee")
            .Select(x => new Employee
            {
                Id = SafeGetInt(x, "Id"),
                Name = SafeGetString(x, "Name"),
                Role = SafeGetString(x, "Role"),
                Email = SafeGetString(x, "Email"),
                IsDeleted = SafeGetBool(x, "IsDeleted")
            }).ToList();

        await _context.BulkInsertOrUpdateAsync(entities, new BulkConfig
        {
            UpdateByProperties = new List<string> { nameof(Employee.Id) }
        });
    }

    private async Task ImportWarehousesAsync(XDocument xml)
    {
        var entities = xml.Descendants("Warehouse")
            .Select(x => new Warehouse
            {
                Id = SafeGetInt(x, "Id"),
                Name = SafeGetString(x, "Name"),
                Location = SafeGetString(x, "Location"),
                ContactPerson = SafeGetString(x, "ContactPerson"),
                Phone = SafeGetString(x, "Phone"),
                IsDeleted = SafeGetBool(x, "IsDeleted")
            }).ToList();

        await _context.BulkInsertOrUpdateAsync(entities, new BulkConfig
        {
            UpdateByProperties = new List<string> { nameof(Warehouse.Id) }
        });
    }

    private async Task ImportProductsAsync(XDocument xml)
    {
        var entities = xml.Descendants("Product")
            .Select(x => new Product
            {
                Id = SafeGetInt(x, "Id"),
                ProductName = SafeGetString(x, "ProductName"),
                SKU = SafeGetString(x, "SKU"),
                WeightKg = SafeGetInt(x, "WeightKg"),
                Material = SafeGetString(x, "Material"),
                BatchNumber = SafeGetInt(x, "BatchNumber"),
                Price = SafeGetDouble(x, "Price"),
                Category = SafeGetString(x, "Category"),
                ExpirationDate = SafeGetNullableDateTime(x, "ExpirationDate"),
                IsDeleted = SafeGetBool(x, "IsDeleted")
            }).ToList();

        await _context.BulkInsertOrUpdateAsync(entities, new BulkConfig
        {
            UpdateByProperties = new List<string> { nameof(Product.Id) }
        });
    }

    private async Task ImportVehiclesAsync(XDocument xml)
    {
        var entities = xml.Descendants("Vehicle")
            .Select(x => new Vehicle
            {
                Id = SafeGetInt(x, "Id"),
                LicensePlate = SafeGetString(x, "LicensePlate"),
                CapacityKg = SafeGetInt(x, "CapacityKg"),
                Type = SafeGetEnum<VehicleType>(x, "Type"),
                Status = SafeGetEnum<VehicleStatus>(x, "Status"),
                IsDeleted = SafeGetBool(x, "IsDeleted")
            }).ToList();

        await _context.BulkInsertOrUpdateAsync(entities, new BulkConfig
        {
            UpdateByProperties = new List<string> { nameof(Vehicle.Id) }
        });
    }

    private async Task ImportOrdersAsync(XDocument xml)
    {
        var entities = xml.Descendants("Order")
            .Select(x => new Order
            {
                Id = SafeGetInt(x, "Id"),
                CustomerId = SafeGetInt(x, "CustomerId"),
                Status = SafeGetEnum<OrderStatus>(x, "Status"),
                OrderDate = SafeGetDateTime(x, "OrderDate"),
                DeliveryAddress = SafeGetString(x, "DeliveryAddress"),
                ExpectedDeliveryDate = SafeGetDateTime(x, "ExpectedDeliveryDate"),
                ActualDeliveryDate = SafeGetNullableDateTime(x, "ActualDeliveryDate"),
                IsDeleted = SafeGetBool(x, "IsDeleted")
            }).ToList();

        await _context.BulkInsertOrUpdateAsync(entities, new BulkConfig
        {
            UpdateByProperties = new List<string> { nameof(Order.Id) }
        });
    }

    private async Task ImportOrderLinesAsync(XDocument xml)
    {
        var entities = xml.Descendants("OrderLine")
            .Select(x => new OrderLine
            {
                Id = SafeGetInt(x, "Id"),
                OrderId = SafeGetInt(x, "OrderId"),
                ProductId = SafeGetInt(x, "ProductId"),
                Quantity = SafeGetInt(x, "Quantity"),
                IsDeleted = SafeGetBool(x, "IsDeleted")
            }).ToList();

        await _context.BulkInsertOrUpdateAsync(entities, new BulkConfig
        {
            UpdateByProperties = new List<string> { nameof(OrderLine.Id) }
        });
    }

    private async Task ImportShipmentsAsync(XDocument xml)
    {
        var entities = xml.Descendants("Shipment")
            .Select(x => new Shipment
            {
                Id = SafeGetInt(x, "Id"),
                VehicleId = SafeGetInt(x, "VehicleId"),
                DriverId = SafeGetInt(x, "DriverId"),
                Status = SafeGetEnum<ShipmentStatus>(x, "Status"),
                DepartureDate = SafeGetDateTime(x, "DepartureDate"),
                ExpectedDeliveryDate = SafeGetDateTime(x, "ExpectedDeliveryDate"),
                ActualDeliveryDate = SafeGetNullableDateTime(x, "ActualDeliveryDate"),
                IsDeleted = SafeGetBool(x, "IsDeleted")
            }).ToList();

        await _context.BulkInsertOrUpdateAsync(entities, new BulkConfig
        {
            UpdateByProperties = new List<string> { nameof(Shipment.Id) }
        });
    }

    private async Task ImportShipmentOrdersAsync(XDocument xml)
    {
        var entities = xml.Descendants("ShipmentOrder")
            .Select(x => new ShipmentOrder
            {
                ShipmentId = SafeGetInt(x, "ShipmentId"),
                OrderId = SafeGetInt(x, "OrderId")
            }).ToList();

        await _context.BulkInsertOrUpdateAsync(entities, new BulkConfig
        {
            UpdateByProperties = new List<string> { nameof(ShipmentOrder.ShipmentId), nameof(ShipmentOrder.OrderId) }
        });
    }

    private async Task ImportInventoryAsync(XDocument xml)
    {
        var entities = xml.Descendants("Inventory")
            .Select(x => new Inventory
            {
                Id = SafeGetInt(x, "Id"),
                ProductId = SafeGetInt(x, "ProductId"),
                WarehouseId = SafeGetInt(x, "WarehouseId"),
                QuantityOnHand = SafeGetInt(x, "QuantityOnHand"),
                LastUpdated = SafeGetDateTime(x, "LastUpdated"),
                IsDeleted = SafeGetBool(x, "IsDeleted")
            }).ToList();

        await _context.BulkInsertOrUpdateAsync(entities, new BulkConfig
        {
            UpdateByProperties = new List<string> { nameof(Inventory.Id) }
        });
    }

    private async Task ImportInventoryTransactionsAsync(XDocument xml)
    {
        var entities = xml.Descendants("InventoryTransaction")
            .Select(x => new InventoryTransaction
            {
                Id = SafeGetInt(x, "Id"),
                ProductId = SafeGetInt(x, "ProductId"),
                Quantity = SafeGetInt(x, "Quantity"),
                Type = SafeGetEnum<InventoryTransactionType>(x, "Type"),
                Timestamp = SafeGetDateTime(x, "Timestamp"),
                EmployeeId = SafeGetInt(x, "EmployeeId"),
                SourceOrDestination = SafeGetString(x, "SourceOrDestination"),
                IsDeleted = SafeGetBool(x, "IsDeleted")
            }).ToList();

        await _context.BulkInsertOrUpdateAsync(entities, new BulkConfig
        {
            UpdateByProperties = new List<string> { nameof(InventoryTransaction.Id) }
        });
    }

    // ===== HELPER METHODS =====
    private string SafeGetString(XElement el, string name, string def = "") 
        => el.Element(name)?.Value ?? def;

    private int SafeGetInt(XElement el, string name, int def = 0)
        => int.TryParse(el.Element(name)?.Value, out int val) ? val : def;

    private double SafeGetDouble(XElement el, string name, double def = 0)
        => double.TryParse(el.Element(name)?.Value, out double val) ? val : def;

    private bool SafeGetBool(XElement el, string name, bool def = false)
        => bool.TryParse(el.Element(name)?.Value, out bool val) ? val : def;

    private DateTime SafeGetDateTime(XElement el, string name, DateTime? def = null)
        => DateTime.TryParse(el.Element(name)?.Value, out DateTime val) 
            ? val : def ?? DateTime.MinValue;

    private DateTime? SafeGetNullableDateTime(XElement el, string name)
        => DateTime.TryParse(el.Element(name)?.Value, out DateTime val) ? val : null;

    private TEnum SafeGetEnum<TEnum>(XElement el, string name) where TEnum : struct
        => Enum.TryParse<TEnum>(el.Element(name)?.Value, out TEnum val) 
            ? val : default;
}