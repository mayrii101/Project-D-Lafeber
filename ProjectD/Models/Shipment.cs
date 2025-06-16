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

        // Navigation property for many-to-many relationship with Order
        public ICollection<ShipmentOrder> ShipmentOrders { get; set; } = new List<ShipmentOrder>();

        [NotMapped]
        public ICollection<Order> Orders => ShipmentOrders.Select(so => so.Order).ToList();
    }

    // ShipmentOrder Model (many-to-many relationship between Shipment and Order)
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
        public DateTime DepartureDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public List<int> OrderIds { get; set; } = new();
    }
}