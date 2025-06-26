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

}