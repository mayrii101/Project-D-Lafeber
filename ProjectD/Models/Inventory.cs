using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Inventory
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; } = default!;

    [Required]
    public int WarehouseId { get; set; }

    [ForeignKey("WarehouseId")]
    public Warehouse Warehouse { get; set; } = default!;

    public int QuantityOnHand { get; set; }

    public DateTime LastUpdated { get; set; }
    public bool IsDeleted { get; set; }
}