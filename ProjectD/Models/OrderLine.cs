using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProjectD.Models
{
    public class OrderLine
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; } = default!;

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = default!;

        public int Quantity { get; set; }

        [NotMapped]
        public double LineTotal => Product.Price * Quantity;
        public bool IsDeleted { get; set; }
    }
}