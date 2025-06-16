using Microsoft.AspNetCore.Mvc;
using ProjectD.Models;
using ProjectD.Services;



namespace AzureSqlConnectionDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            var result = orders.Select(o => new
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
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            var result = new
            {
                order.Id,
                Customer = new
                {
                    order.Customer.Id,
                    order.Customer.BedrijfsNaam,
                    order.Customer.ContactPersoon,
                    order.Customer.Email,
                    order.Customer.TelefoonNummer,
                    order.Customer.Adres
                },
                ProductLines = order.ProductLines
                    .Where(pl => !pl.IsDeleted)
                    .Select(pl => new
                    {
                        ProductName = pl.Product.ProductName,
                        Quantity = pl.Quantity,
                        LineTotal = pl.LineTotal,
                        Material = pl.Product.Material
                    }),
                order.TotalWeight,
                order.Status,
                order.OrderDate,
                order.DeliveryAddress,
                order.ExpectedDeliveryDate,
                ShipmentId = order.ShipmentOrders
                    .Where(so => !so.Shipment.IsDeleted)
                    .Select(so => so.ShipmentId)
                    .FirstOrDefault()
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderCreateDto dto)
        {
            var createdOrder = await _orderService.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Order>> UpdateOrder(int id, Order order)
        {
            var updatedOrder = await _orderService.UpdateOrderAsync(id, order);
            return updatedOrder == null ? NotFound() : Ok(updatedOrder);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftDeleteOrder(int id)
        {
            var deleted = await _orderService.SoftDeleteOrderAsync(id);
            return !deleted ? NotFound() : NoContent();
        }
    }
}
