using Microsoft.EntityFrameworkCore;
using ProjectD.Models;


namespace ProjectD.Services
{

    public interface IShipmentService
    {
        Task<List<ShipmentDto>> GetAllShipmentsAsync();
        Task<ShipmentDto> GetShipmentByIdAsync(int id);
        Task<ShipmentDto> CreateShipmentAsync(ShipmentCreateDto dto);
        Task<Shipment> UpdateShipmentAsync(int id, Shipment shipment);
        Task<bool> SoftDeleteShipmentAsync(int id);  // Soft delete instead of hard delete
    }



    public class ShipmentService : IShipmentService
    {
        private readonly ApplicationDbContext _context;

        public ShipmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ShipmentDto>> GetAllShipmentsAsync()
        {
            var shipments = await _context.Shipments
                .Where(s => !s.IsDeleted)
                .Include(s => s.ShipmentOrders)
                    .ThenInclude(so => so.Order)
                .ToListAsync();

            return shipments.Select(s => new ShipmentDto
            {
                Id = s.Id,
                VehicleId = s.VehicleId,
                DriverId = s.DriverId,
                Status = s.Status,
                DepartureDate = s.DepartureDate,
                ExpectedDeliveryDate = s.ExpectedDeliveryDate,
                OrderIds = s.ShipmentOrders.Select(so => so.OrderId).ToList()
            }).ToList();
        }

        public async Task<ShipmentDto> GetShipmentByIdAsync(int id)
        {
            var shipment = await _context.Shipments
                .Where(s => !s.IsDeleted && s.Id == id)
                .Include(s => s.ShipmentOrders)
                    .ThenInclude(so => so.Order)
                .FirstOrDefaultAsync();

            if (shipment == null) return null;

            return new ShipmentDto
            {
                Id = shipment.Id,
                VehicleId = shipment.VehicleId,
                DriverId = shipment.DriverId,
                Status = shipment.Status,
                DepartureDate = shipment.DepartureDate,
                ExpectedDeliveryDate = shipment.ExpectedDeliveryDate,
                OrderIds = shipment.ShipmentOrders.Select(so => so.OrderId).ToList()
            };
        }

        private DateTime ParseDateTime(string date, string time)
        {
            var datePart = DateTime.ParseExact(date, "dd-MM-yyyy", null);
            var timePart = TimeSpan.Parse(time);
            return datePart.Date + timePart;
        }

        public async Task<ShipmentDto> CreateShipmentAsync(ShipmentCreateDto dto)
        {
            // Fetch vehicle to get max capacity
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == dto.VehicleId && !v.IsDeleted);

            if (vehicle == null)
            {
                throw new InvalidOperationException("Voertuig niet gevonden."); // Vehicle not found
            }

            // Fetch all orders in dto.OrderIds including their ProductLines for weight
            var orders = await _context.Orders
                .Where(o => dto.OrderIds.Contains(o.Id) && !o.IsDeleted)
                .Include(o => o.ProductLines)
                    .ThenInclude(pl => pl.Product)
                .ToListAsync();

            // Calculate total weight of all orders combined
            int totalWeight = orders.Sum(o => o.TotalWeight);

            if (totalWeight > vehicle.CapacityKg)
            {
                throw new InvalidOperationException(
                    $"Het totale gewicht van de bestelling ({totalWeight} kg) overschrijdt de maximale capaciteit van het voertuig ({vehicle.CapacityKg} kg). Kies een voertuig met een hogere capaciteit."
                );
            }

            var departureDateTime = ParseDateTime(dto.DepartureDate, dto.DepartureTime);
            DateTime? expectedDeliveryDateTime = null;

            if (!string.IsNullOrEmpty(dto.ExpectedDeliveryDate) && !string.IsNullOrEmpty(dto.ExpectedDeliveryTime))
            {
                expectedDeliveryDateTime = ParseDateTime(dto.ExpectedDeliveryDate, dto.ExpectedDeliveryTime);
            }

            var shipment = new Shipment
            {
                VehicleId = dto.VehicleId,
                DriverId = dto.DriverId,
                Status = dto.Status,
                DepartureDate = departureDateTime,
                ExpectedDeliveryDate = expectedDeliveryDateTime,
                ShipmentOrders = dto.OrderIds
                    .Select(orderId => new ShipmentOrder { OrderId = orderId })
                    .ToList()
            };

            foreach (var order in orders)
            {
                order.Status = OrderStatus.Shipped;
            }

            vehicle.Status = VehicleStatus.InUse;

            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();

            return new ShipmentDto
            {
                Id = shipment.Id,
                VehicleId = shipment.VehicleId,
                DriverId = shipment.DriverId,
                Status = shipment.Status,
                DepartureDate = shipment.DepartureDate,
                ExpectedDeliveryDate = shipment.ExpectedDeliveryDate,
                OrderIds = shipment.ShipmentOrders.Select(so => so.OrderId).ToList()
            };
        }


        public async Task<Shipment> UpdateShipmentAsync(int id, Shipment shipment)
        {
            var existingShipment = await _context.Shipments
                .Include(s => s.Orders)  //related orders
                .Include(s => s.Vehicle) //related vehicle
                .Include(s => s.Driver)  //related driver
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingShipment == null || existingShipment.IsDeleted) return null;

            //shipment properties
            existingShipment.Vehicle = shipment.Vehicle;  //related vehicle object
            existingShipment.Driver = shipment.Driver;    //related driver object
            existingShipment.Status = shipment.Status;
            existingShipment.DepartureDate = shipment.DepartureDate;
            existingShipment.ExpectedDeliveryDate = shipment.ExpectedDeliveryDate;
            existingShipment.ActualDeliveryDate = shipment.ActualDeliveryDate;

            //orders in the shipment (assuming you want to add a new order to the shipment)
            foreach (var updatedOrder in shipment.Orders)
            {
                var existingOrder = existingShipment.Orders
                    .FirstOrDefault(o => o.Id == updatedOrder.Id);

                if (existingOrder == null)
                {

                    existingShipment.Orders.Add(updatedOrder);
                }
            }

            await _context.SaveChangesAsync();
            return existingShipment;
        }

        public async Task<bool> SoftDeleteShipmentAsync(int id)
        {
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null || shipment.IsDeleted) return false;

            shipment.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}