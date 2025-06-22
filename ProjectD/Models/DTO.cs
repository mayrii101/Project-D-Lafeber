using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectD.Models;
namespace ProjectD.Models
{

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
