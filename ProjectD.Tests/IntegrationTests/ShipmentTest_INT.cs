/*
using Xunit;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
using AzureSqlConnectionDemo.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class ShipmentIntegrationTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    private ShipmentController GetController(ApplicationDbContext context)
    {
        var service = new ShipmentService(context);
        return new ShipmentController(service);
    }

    private Shipment CreateTestShipment()
    {
        return new Shipment
        {
            VehicleId = 1,
            DriverId = 1,
            Status = ShipmentStatus.Preparing,
            DepartureDate = DateTime.UtcNow,
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(3),
            IsDeleted = false
        };
    }

    [Fact]
    public async Task Can_Create_And_Get_Shipment()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var shipment = CreateTestShipment();

        var createResult = await controller.CreateShipment(shipment);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Shipment;

        var getResult = await controller.GetShipment(created.Id);
        var fetched = (getResult.Result as OkObjectResult)?.Value as Shipment;

        Assert.NotNull(created);
        Assert.NotNull(fetched);
        Assert.Equal(shipment.VehicleId, fetched.VehicleId);
        Assert.Equal(shipment.DriverId, fetched.DriverId);
        Assert.Equal(shipment.Status, fetched.Status);
    }

    // ... other methods ...
}
*/
