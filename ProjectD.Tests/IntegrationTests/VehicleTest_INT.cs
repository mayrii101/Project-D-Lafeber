using Xunit;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
using AzureSqlConnectionDemo.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

public class VehicleIntegrationTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    private VehicleController GetController(ApplicationDbContext context)
    {
        var service = new VehicleService(context);
        return new VehicleController(service);
    }

    private Vehicle CreateTestVehicle()
    {
        return new Vehicle
        {
            LicensePlate = "ABC123",
            CapacityKg = 1500,
            Type = VehicleType.FlatbedTrailer,
            Status = VehicleStatus.Available,
            IsDeleted = false
        };
    }

    [Fact]
    public async Task Can_Create_And_Get_Vehicle()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var vehicle = CreateTestVehicle();

        var createResult = await controller.CreateVehicle(vehicle);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Vehicle;

        var getResult = await controller.GetVehicle(created.Id);
        var fetched = (getResult.Result as OkObjectResult)?.Value as Vehicle;

        Assert.NotNull(created);
        Assert.NotNull(fetched);
        Assert.Equal(vehicle.LicensePlate, fetched.LicensePlate);
        Assert.Equal(vehicle.CapacityKg, fetched.CapacityKg);
    }

    [Fact]
    public async Task Can_Update_Vehicle()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var vehicle = CreateTestVehicle();
        var createResult = await controller.CreateVehicle(vehicle);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Vehicle;

        Assert.NotNull(created);

        // Modify properties
        created.Status = VehicleStatus.Maintenance;
        created.Type = VehicleType.FlatbedTrailer;

        var updateResult = await controller.UpdateVehicle(created.Id, created);
        var updated = (updateResult.Result as OkObjectResult)?.Value as Vehicle;

        Assert.NotNull(updated);
        Assert.Equal(VehicleStatus.Maintenance, updated.Status);
        Assert.Equal(VehicleType.FlatbedTrailer, updated.Type);
    }

    [Fact]
    public async Task Can_SoftDelete_Vehicle()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var vehicle = CreateTestVehicle();
        var createResult = await controller.CreateVehicle(vehicle);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Vehicle;

        var deleteResult = await controller.SoftDeleteVehicle(created.Id);
        Assert.IsType<NoContentResult>(deleteResult);

        var getResult = await controller.GetVehicle(created.Id);
        Assert.IsType<NotFoundResult>(getResult.Result);
    }

    [Fact]
    public async Task GetAll_Returns_Only_NotDeleted_Vehicles()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        await controller.CreateVehicle(CreateTestVehicle());

        var toDelete = CreateTestVehicle();
        var create = await controller.CreateVehicle(toDelete);
        var created = (create.Result as CreatedAtActionResult)?.Value as Vehicle;
        await controller.SoftDeleteVehicle(created.Id);

        var getAllResult = await controller.GetAllVehicles();
        var okResult = getAllResult.Result as OkObjectResult;
        var vehicles = okResult?.Value as List<Vehicle>;

        Assert.Single(vehicles);
        Assert.DoesNotContain(vehicles, v => v.Id == created.Id);
    }
}
