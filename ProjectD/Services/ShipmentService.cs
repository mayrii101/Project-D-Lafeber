using Microsoft.EntityFrameworkCore;
using ProjectD.Models;

namespace ProjectD.Services
{

    public interface IShipmentService
    {
        Task<List<Shipment>> GetAllShipmentsAsync();
        Task<Shipment> GetShipmentByIdAsync(int id);
        Task<Shipment> CreateShipmentAsync(ShipmentCreateDto dto);
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

        public async Task<List<Shipment>> GetAllShipmentsAsync()
        {
            return await _context.Shipments.Where(s => !s.IsDeleted).ToListAsync();
        }

        public async Task<Shipment> GetShipmentByIdAsync(int id)
        {
            return await _context.Shipments.Where(s => !s.IsDeleted).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Shipment> CreateShipmentAsync(ShipmentCreateDto dto)
        {
            var shipment = new Shipment
            {
                VehicleId = dto.VehicleId,
                DriverId = dto.DriverId,
                Status = dto.Status,
                DepartureDate = dto.DepartureDate,
                ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
                ShipmentOrders = dto.OrderIds
                    .Select(orderId => new ShipmentOrder { OrderId = orderId })
                    .ToList()
            };

            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();

            return shipment;
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

        public bool CreateShipmentOrder(int orderId, int shipmentId)
        {
            var order = _context.Orders.Find(orderId);
            var shipment = _context.Shipments.Find(shipmentId);

            if (order != null && shipment != null)
            {
                var shipmentOrder = new ShipmentOrder
                {
                    Shipment = shipment,
                    Order = order
                };

                _context.ShipmentOrders.Add(shipmentOrder);
                _context.SaveChanges();
                return true;
            }

            return false;
        }
    }
}