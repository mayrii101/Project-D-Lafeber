using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectD.Models;

namespace ProjectD.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = default!;

        public List<OrderLine> ProductLines { get; set; } = new();

        [NotMapped]
        public int TotalWeight => ProductLines.Sum(pl => pl.Product.WeightKg * pl.Quantity);

        public OrderStatus Status { get; set; }

        public DateTime OrderDate { get; set; }

        public string DeliveryAddress { get; set; } = string.Empty;

        public DateTime ExpectedDeliveryDate { get; set; }

        public DateTime? ActualDeliveryDate { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<ShipmentOrder> ShipmentOrders { get; set; } = new List<ShipmentOrder>();

    }

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
}