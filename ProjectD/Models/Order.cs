using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AzureSqlConnectionDemo.Models;


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