using System;
using System.Collections.Generic;
using System.Linq; // Make sure to include this for LINQ
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectD.Models
{
    // Shipment Model
    public class Shipment
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public Vehicle? Vehicle { get; set; } // Navigation property
        public Employee? Driver { get; set; }  // Navigation property
        public ShipmentStatus Status { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<ShipmentOrder> ShipmentOrders { get; set; } = new List<ShipmentOrder>();

        [NotMapped]
        public ICollection<Order> Orders => ShipmentOrders.Select(so => so.Order).ToList();
    }

    public class ShipmentOrder
    {
        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
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