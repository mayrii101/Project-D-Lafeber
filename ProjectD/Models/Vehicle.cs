using System.ComponentModel.DataAnnotations;

namespace ProjectD.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        public string LicensePlate { get; set; } = string.Empty;

        public int CapacityKg { get; set; }

        public VehicleType Type { get; set; }

        public VehicleStatus Status { get; set; }
        public bool IsDeleted { get; set; }
    }
}