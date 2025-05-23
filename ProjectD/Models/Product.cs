using System.ComponentModel.DataAnnotations;
namespace ProjectD.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public int WeightKg { get; set; }

        public string Material { get; set; } = string.Empty;

        public int BatchNumber { get; set; }

        [Range(0, double.MaxValue)]
        public double Price { get; set; }

        public string Category { get; set; } = string.Empty;

        public DateTime? ExpirationDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}