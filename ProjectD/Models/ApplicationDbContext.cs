using Microsoft.EntityFrameworkCore;
using ProjectD.Models;

namespace ProjectD.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<ShipmentOrder> ShipmentOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for ShipmentOrder
            modelBuilder.Entity<ShipmentOrder>()
                .HasKey(so => new { so.ShipmentId, so.OrderId });

            // Define relationships
            modelBuilder.Entity<ShipmentOrder>()
                .HasOne(so => so.Shipment)
                .WithMany(s => s.ShipmentOrders)
                .HasForeignKey(so => so.ShipmentId);

            modelBuilder.Entity<ShipmentOrder>()
                .HasOne(so => so.Order)
                .WithMany(o => o.ShipmentOrders)
                .HasForeignKey(so => so.OrderId);
        }
    }
}
