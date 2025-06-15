using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectD.Models;
using System.Linq.Expressions;
using EFCore.BulkExtensions;

namespace ProjectD.Services
{
    public class XmlToSqlImporter
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<XmlToSqlImporter> _logger;

        public XmlToSqlImporter(ApplicationDbContext context, ILogger<XmlToSqlImporter> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ImportFromXmlStringAsync(string xmlContent)
        {
            _logger.LogInformation("Starting import from single XML string.");
            var xml = XDocument.Parse(xmlContent);

            bool supportsTransactions = !_context.Database.ProviderName.Equals("Microsoft.EntityFrameworkCore.InMemory", StringComparison.OrdinalIgnoreCase);

            if (supportsTransactions)
            {
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
                    _logger.LogInformation("Data imported successfully from single XML string with transaction.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Import unsuccessful, transaction rolled back.");
                    throw;
                }
            }
            else
            {
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

                    _logger.LogInformation("Data imported successfully from single XML string without transaction.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Import unsuccessful without transaction.");
                    throw;
                }
            }
        }

        // ===== INDIVIDUAL IMPORT METHODS =====
        private async Task ImportCustomersAsync(XDocument xml)
        {
            _logger.LogInformation("Importing Customers...");
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
            _logger.LogInformation("Parsed {Count} Customers from XML.", entities.Count);

            await UpsertEntitiesAsync(entities, c => c.Id);
            _logger.LogInformation("Imported Customers.");
        }

        private async Task ImportEmployeesAsync(XDocument xml)
        {
            _logger.LogInformation("Importing Employees...");
            var entities = xml.Descendants("Employee")
                .Select(x => new Employee
                {
                    Id = SafeGetInt(x, "Id"),
                    Name = SafeGetString(x, "Name"),
                    Role = SafeGetString(x, "Role"),
                    Email = SafeGetString(x, "Email"),
                    IsDeleted = SafeGetBool(x, "IsDeleted")
                }).ToList();
            _logger.LogInformation("Parsed {Count} Employees from XML.", entities.Count);

            await UpsertEntitiesAsync(entities, e => e.Id);
            _logger.LogInformation("Imported Employees.");
        }

        private async Task ImportWarehousesAsync(XDocument xml)
        {
            _logger.LogInformation("Importing Warehouses...");
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
            _logger.LogInformation("Parsed {Count} Warehouses from XML.", entities.Count);

            await UpsertEntitiesAsync(entities, w => w.Id);
            _logger.LogInformation("Imported Warehouses.");
        }

        private async Task ImportProductsAsync(XDocument xml)
        {
            _logger.LogInformation("Importing Products...");
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
            _logger.LogInformation("Parsed {Count} Products from XML.", entities.Count);

            await UpsertEntitiesAsync(entities, p => p.Id);
            _logger.LogInformation("Imported Products.");
        }

        private async Task ImportVehiclesAsync(XDocument xml)
        {
            _logger.LogInformation("Importing Vehicles...");
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
            _logger.LogInformation("Parsed {Count} Vehicles from XML.", entities.Count);

            await UpsertEntitiesAsync(entities, v => v.Id);
            _logger.LogInformation("Imported Vehicles.");
        }

        private async Task ImportOrdersAsync(XDocument xml)
        {
            _logger.LogInformation("Importing Orders...");
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
            _logger.LogInformation("Parsed {Count} Orders from XML.", entities.Count);

            await UpsertEntitiesAsync(entities, o => o.Id);
            _logger.LogInformation("Imported Orders.");
        }

        private async Task ImportOrderLinesAsync(XDocument xml)
        {
            _logger.LogInformation("Importing OrderLines...");
            var entities = xml.Descendants("OrderLine")
                .Select(x => new OrderLine
                {
                    Id = SafeGetInt(x, "Id"),
                    OrderId = SafeGetInt(x, "OrderId"),
                    ProductId = SafeGetInt(x, "ProductId"),
                    Quantity = SafeGetInt(x, "Quantity"),
                    IsDeleted = SafeGetBool(x, "IsDeleted")
                }).ToList();
            _logger.LogInformation("Parsed {Count} OrderLines from XML.", entities.Count);

            await UpsertEntitiesAsync(entities, ol => ol.Id);
            _logger.LogInformation("Imported OrderLines.");
        }

        private async Task ImportShipmentsAsync(XDocument xml)
        {
            _logger.LogInformation("Importing Shipments...");
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
            _logger.LogInformation("Parsed {Count} Shipments from XML.", entities.Count);

            await UpsertEntitiesAsync(entities, s => s.Id);
            _logger.LogInformation("Imported Shipments.");
        }

        private async Task ImportShipmentOrdersAsync(XDocument xml)
        {
            _logger.LogInformation("Importing ShipmentOrders...");
            var entities = xml.Descendants("ShipmentOrder")
                .Select(x => new ShipmentOrder
                {
                    ShipmentId = SafeGetInt(x, "ShipmentId"),
                    OrderId = SafeGetInt(x, "OrderId")
                }).ToList();
            _logger.LogInformation("Parsed {Count} ShipmentOrders from XML.", entities.Count);

            foreach (var entity in entities)
            {
                var existing = await _context.ShipmentOrders
                    .FirstOrDefaultAsync(so => so.ShipmentId == entity.ShipmentId && so.OrderId == entity.OrderId);

                if (existing == null)
                {
                    _context.ShipmentOrders.Add(entity);
                    _logger.LogDebug("Added new ShipmentOrder ShipmentId={ShipmentId} OrderId={OrderId}", entity.ShipmentId, entity.OrderId);
                }
                else
                {
                    _logger.LogDebug("Skipped existing ShipmentOrder ShipmentId={ShipmentId} OrderId={OrderId}", entity.ShipmentId, entity.OrderId);
                }
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Imported ShipmentOrders.");
        }

        private async Task ImportInventoryAsync(XDocument xml)
        {
            _logger.LogInformation("Importing Inventory...");
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
            _logger.LogInformation("Parsed {Count} Inventory records from XML.", entities.Count);

            await UpsertEntitiesAsync(entities, i => i.Id);
            _logger.LogInformation("Imported Inventory.");
        }

        private async Task ImportInventoryTransactionsAsync(XDocument xml)
        {
            _logger.LogInformation("Importing InventoryTransactions...");
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
            _logger.LogInformation("Parsed {Count} InventoryTransactions from XML.", entities.Count);

            await UpsertEntitiesAsync(entities, it => it.Id);
            _logger.LogInformation("Imported InventoryTransactions.");
        }


        // Generic upsert method: tries to update if exists by key, otherwise add new
        private async Task UpsertEntitiesAsync<TEntity, TKey>(List<TEntity> entities, Func<TEntity, TKey> keySelector) where TEntity : class
        {
            var keys = entities.Select(keySelector).ToList();
            var dbSet = _context.Set<TEntity>();

            var allEntities = await dbSet.ToListAsync(); // fetch all and filter in memory
            var existingEntities = allEntities
                .Where(e => keys.Contains(keySelector(e)))
                .ToList();

            foreach (var entity in entities)
            {
                try
                {
                    var key = keySelector(entity);
                    var existing = existingEntities.FirstOrDefault(e => keySelector(e).Equals(key));
                    if (existing != null)
                    {
                        _context.Entry(existing).CurrentValues.SetValues(entity);
                        _logger.LogDebug("Updated existing entity of type {EntityType} with key {Key}", typeof(TEntity).Name, key);
                    }
                    else
                    {
                        await dbSet.AddAsync(entity);
                        _logger.LogDebug("Added new entity of type {EntityType} with key {Key}", typeof(TEntity).Name, key);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error upserting entity of type {EntityType}", typeof(TEntity).Name);
                    throw;
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Saved changes for {Count} entities of type {EntityType}.", entities.Count, typeof(TEntity).Name);
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
}