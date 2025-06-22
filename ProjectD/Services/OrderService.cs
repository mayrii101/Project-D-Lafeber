using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using System.Net;
namespace ProjectD.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<OrderCreateDto> CreateOrderAsync(OrderCreateDto dto);
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
        public async Task<OrderCreateDto> CreateOrderAsync(OrderCreateDto dto)
        {
            var orderDateTime = ParseDateTime(dto.OrderDate, dto.OrderTime);
            var expectedDeliveryDateTime = ParseDateTime(dto.ExpectedDeliveryDate, dto.ExpectedDeliveryTime);

            // Step 1: Inventory check
            foreach (var productLine in dto.ProductLines)
            {
                int totalAvailable = await _context.Inventories
                    .Where(i => i.ProductId == productLine.ProductId && !i.IsDeleted)
                    .SumAsync(i => (int?)i.QuantityOnHand) ?? 0;

                if (totalAvailable < productLine.Quantity)
                {
                    throw new HttpRequestException($"Niet genoeg voorraad voor product {productLine.ProductId}", null, HttpStatusCode.UnprocessableEntity);
                }
            }

            // Step 2: Create order
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

            // Step 3: Deduct inventory
            foreach (var productLine in dto.ProductLines)
            {
                int quantityToDeduct = productLine.Quantity;

                var inventories = await _context.Inventories
                    .Where(i => i.ProductId == productLine.ProductId && !i.IsDeleted && i.QuantityOnHand > 0)
                    .OrderBy(i => i.LastUpdated)
                    .ToListAsync();

                foreach (var inventory in inventories)
                {
                    if (quantityToDeduct <= 0) break;

                    int deduct = Math.Min(quantityToDeduct, inventory.QuantityOnHand);
                    inventory.QuantityOnHand -= deduct;
                    inventory.LastUpdated = DateTime.UtcNow;

                    quantityToDeduct -= deduct;

                    // Force EF to track it (just in case)
                    _context.Entry(inventory).State = EntityState.Modified;
                }
            }

            // Step 4: Commit changes
            await _context.SaveChangesAsync();

            // Step 5: Return full order
            var productIds = dto.ProductLines.Select(p => p.ProductId).ToList();

            var productStocks = await _context.Inventories
                .Where(i => productIds.Contains(i.ProductId) && !i.IsDeleted)
                .GroupBy(i => i.ProductId)
                .Select(g => new ProductStockDto
                {
                    ProductId = g.Key,
                    RemainingStock = g.Sum(i => i.QuantityOnHand)
                })
                .ToListAsync();


            dto.ProductStocks = productStocks;
            dto.Id = order.Id;  // set the newly created order ID
            dto.Message = "Bestelling geplaatst!.";
            dto.ProductStocks = productStocks;

            return dto;
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