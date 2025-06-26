using System.ComponentModel.DataAnnotations;
namespace ProjectD.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}