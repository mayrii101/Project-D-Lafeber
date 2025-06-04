using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
namespace ProjectD.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> UpdateOrderAsync(int id, Order order);
        Task<bool> SoftDeleteOrderAsync(int id);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Where(o => !o.IsDeleted)
                .Select(o => new
                {
                    o.Id,
                    Customer = new
                    {
                        o.Customer.Id,
                        o.Customer.BedrijfsNaam,
                        o.Customer.ContactPersoon,
                        o.Customer.Email,
                        o.Customer.TelefoonNummer,
                        o.Customer.Adres
                    },
                    ProductLines = o.ProductLines
                        .Where(pl => !pl.IsDeleted)
                        .Select(pl => new
                        {
                            ProductName = pl.Product.ProductName,
                            Quantity = pl.Quantity,
                            LineTotal = pl.LineTotal,
                            Material = pl.Product.Material
                        }),
                    o.TotalWeight,
                    o.Status,
                    o.OrderDate,
                    o.DeliveryAddress,
                    o.ExpectedDeliveryDate,
                    ShipmentId = o.ShipmentOrders
                        .Where(so => !so.Shipment.IsDeleted)
                        .Select(so => so.ShipmentId)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.ProductLines)
                    .ThenInclude(pl => pl.Product)
                .Include(o => o.ShipmentOrders)
                    .ThenInclude(so => so.Shipment)
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> UpdateOrderAsync(int id, Order order)
        {
            var existingOrder = await GetOrderByIdAsync(id);
            if (existingOrder == null) return null;

            existingOrder.CustomerId = order.CustomerId;
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.DeliveryAddress = order.DeliveryAddress;
            existingOrder.ExpectedDeliveryDate = order.ExpectedDeliveryDate;
            existingOrder.ActualDeliveryDate = order.ActualDeliveryDate;
            existingOrder.Status = order.Status;

            existingOrder.ProductLines = order.ProductLines;

            await _context.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<bool> SoftDeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null || order.IsDeleted) return false;

            order.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}