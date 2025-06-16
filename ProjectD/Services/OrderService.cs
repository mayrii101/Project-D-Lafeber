using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
namespace ProjectD.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(OrderCreateDto dto);
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

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.ProductLines)
                    .ThenInclude(pl => pl.Product)
                .Include(o => o.ShipmentOrders)
                    .ThenInclude(so => so.Shipment)
                .Where(o => !o.IsDeleted)
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

        private DateTime ParseDateTime(string date, string time)
        {
            var datePart = DateTime.ParseExact(date, "dd-MM-yyyy", null);
            var timePart = TimeSpan.Parse(time);
            return datePart.Date + timePart;
        }
        public async Task<Order> CreateOrderAsync(OrderCreateDto dto)
        {
            var orderDateTime = ParseDateTime(dto.OrderDate, dto.OrderTime);
            var expectedDeliveryDateTime = ParseDateTime(dto.ExpectedDeliveryDate, dto.ExpectedDeliveryTime);

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderDate = orderDateTime,
                DeliveryAddress = dto.DeliveryAddress,
                ExpectedDeliveryDate = expectedDeliveryDateTime,
                Status = dto.Status,
                ProductLines = dto.ProductLines.Select(pl => new OrderLine
                {
                    ProductId = pl.ProductId,
                    Quantity = pl.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Reload order with related products
            var createdOrder = await _context.Orders
                .Include(o => o.ProductLines)
                    .ThenInclude(ol => ol.Product)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return createdOrder!;
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