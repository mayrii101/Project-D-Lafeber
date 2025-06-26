namespace ProjectD.Models
{
    public enum InventoryTransactionType
    {
        Inbound,
        Outbound,
        Adjustment
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public enum VehicleType
    {
        FlatbedTrailer,
        LowbedTrailer,
        Kipper,
        SuperTruck
    }

    public enum VehicleStatus
    {
        Available,
        InUse,
        Maintenance
    }

    public enum ShipmentStatus
    {
        Preparing,
        OutForDelivery,
        Delivered
    }

}