using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectD.Models;
namespace ProjectD.Models
{
    public class OrderCreateDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        public string OrderDate { get; set; } = "";
        public string OrderTime { get; set; } = "";

        public string DeliveryAddress { get; set; } = "";

        public string ExpectedDeliveryDate { get; set; } = "";
        public string ExpectedDeliveryTime { get; set; } = "";

        public OrderStatus Status { get; set; }

        public List<OrderLineCreateDto> ProductLines { get; set; } = new();
        public string? Message { get; set; } = null;

        public List<ProductStockDto> ProductStocks { get; set; } = new();
    }

    public class OrderLineCreateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class ProductStockDto
    {
        public int ProductId { get; set; }
        public int RemainingStock { get; set; }
    }

    public class ShipmentCreateDto
    {
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public ShipmentStatus Status { get; set; }

        public string DepartureDate { get; set; } = "";     //30-04-2025
        public string DepartureTime { get; set; } = "";     //14:30

        public string? ExpectedDeliveryDate { get; set; }   //02-05-2025
        public string? ExpectedDeliveryTime { get; set; }   //09:00

        public List<int> OrderIds { get; set; } = new();
    }

    public class ShipmentDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public ShipmentStatus Status { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }

        public List<int> OrderIds { get; set; } = new();
    }
}