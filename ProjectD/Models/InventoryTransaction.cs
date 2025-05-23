using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProjectD.Models
{
    public class InventoryTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = default!;

        public int Quantity { get; set; }

        public InventoryTransactionType Type { get; set; }

        public DateTime Timestamp { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; } = default!;

        public string SourceOrDestination { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}