using Xunit;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
using AzureSqlConnectionDemo.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class WarehouseIntegrationTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    private WarehouseController GetController(ApplicationDbContext context)
    {
        var service = new WarehouseService(context);
        return new WarehouseController(service);
    }

    private Warehouse CreateTestWarehouse()
    {
        return new Warehouse
        {
            Name = "Main Warehouse",
            Location = "123 Warehouse St",
            ContactPerson = "John Doe",
            Phone = "123-456-7890",
            IsDeleted = false
        };
    }

    [Fact]
    public async Task Can_Create_And_Get_Warehouse()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var warehouse = CreateTestWarehouse();

        var createResult = await controller.CreateWarehouse(warehouse);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Warehouse;

        var getResult = await controller.GetWarehouse(created.Id);
        var fetched = (getResult.Result as OkObjectResult)?.Value as Warehouse;

        Assert.NotNull(created);
        Assert.NotNull(fetched);
        Assert.Equal(warehouse.Name, fetched.Name);
        Assert.Equal(warehouse.Location, fetched.Location);
        Assert.Equal(warehouse.ContactPerson, fetched.ContactPerson);
        Assert.Equal(warehouse.Phone, fetched.Phone);
    }

    [Fact]
    public async Task Can_Update_Warehouse()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var warehouse = CreateTestWarehouse();
        var createResult = await controller.CreateWarehouse(warehouse);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Warehouse;

        created.Name = "Updated Warehouse";
        created.Location = "456 New Address";
        created.ContactPerson = "Jane Smith";
        created.Phone = "987-654-3210";

        var updateResult = await controller.UpdateWarehouse(created.Id, created);
        var updated = (updateResult.Result as OkObjectResult)?.Value as Warehouse;

        Assert.NotNull(updated);
        Assert.Equal("Updated Warehouse", updated.Name);
        Assert.Equal("456 New Address", updated.Location);
        Assert.Equal("Jane Smith", updated.ContactPerson);
        Assert.Equal("987-654-3210", updated.Phone);
    }

    [Fact]
    public async Task Can_SoftDelete_Warehouse()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var warehouse = CreateTestWarehouse();
        var createResult = await controller.CreateWarehouse(warehouse);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Warehouse;

        var deleteResult = await controller.SoftDeleteWarehouse(created.Id);
        Assert.IsType<NoContentResult>(deleteResult);

        var getResult = await controller.GetWarehouse(created.Id);
        Assert.IsType<NotFoundResult>(getResult.Result);
    }

    [Fact]
    public async Task GetAll_Returns_Only_NotDeleted_Warehouses()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        // Create one warehouse that remains active
        await controller.CreateWarehouse(CreateTestWarehouse());

        // Create another warehouse and soft delete it
        var toDelete = CreateTestWarehouse();
        var create = await controller.CreateWarehouse(toDelete);
        var created = (create.Result as CreatedAtActionResult)?.Value as Warehouse;
        await controller.SoftDeleteWarehouse(created.Id);

        var getAllResult = await controller.GetAllWarehouses();
        var okResult = getAllResult.Result as OkObjectResult;
        var warehouses = okResult?.Value as List<Warehouse>;

        Assert.Single(warehouses);
        Assert.DoesNotContain(warehouses, w => w.Id == created.Id);
    }
}
