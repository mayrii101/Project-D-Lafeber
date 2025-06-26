using Xunit;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
using AzureSqlConnectionDemo.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class InventoryIntegrationTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    private InventoryController GetController(ApplicationDbContext context)
    {
        var service = new InventoryService(context);
        return new InventoryController(service);
    }

    private Inventory CreateTestInventory()
    {
        return new Inventory
        {
            ProductId = 1,
            WarehouseId = 1,
            QuantityOnHand = 50,
            LastUpdated = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    [Fact]
    public async Task Can_Create_And_Get_Inventory()
    {
        var context = GetInMemoryDbContext();

        context.Products.Add(new Product { Id = 1, ProductName = "TestProduct", Category = "", Price = 10 });
        context.Warehouses.Add(new Warehouse { Id = 1, Name = "Main", Location = "A", Phone = "", ContactPerson = "" });
        await context.SaveChangesAsync();

        var controller = GetController(context);
        var inventory = CreateTestInventory();

        var createResult = await controller.CreateInventory(inventory);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Inventory;

        var getResult = await controller.GetInventory(created.Id);
        var fetched = (getResult.Result as OkObjectResult)?.Value as Inventory;

        Assert.NotNull(created);
        Assert.NotNull(fetched);
        Assert.Equal(inventory.ProductId, fetched.ProductId);
        Assert.Equal(inventory.WarehouseId, fetched.WarehouseId);
    }

    [Fact]
    public async Task Can_Update_Inventory()
    {
        var context = GetInMemoryDbContext();
        context.Products.Add(new Product { Id = 1 });
        context.Warehouses.Add(new Warehouse { Id = 1 });
        await context.SaveChangesAsync();

        var controller = GetController(context);
        var inventory = CreateTestInventory();
        var create = await controller.CreateInventory(inventory);
        var created = (create.Result as CreatedAtActionResult)?.Value as Inventory;

        Assert.NotNull(created);

        created.QuantityOnHand = 100;
        var update = await controller.UpdateInventory(created.Id, created);
        var updated = (update.Result as OkObjectResult)?.Value as Inventory;

        Assert.NotNull(updated);
        Assert.Equal(100, updated.QuantityOnHand);
    }

    [Fact]
    public async Task Can_SoftDelete_Inventory()
    {
        var context = GetInMemoryDbContext();
        context.Products.Add(new Product { Id = 1 });
        context.Warehouses.Add(new Warehouse { Id = 1 });
        await context.SaveChangesAsync();

        var controller = GetController(context);
        var inventory = CreateTestInventory();
        var create = await controller.CreateInventory(inventory);
        var created = (create.Result as CreatedAtActionResult)?.Value as Inventory;

        var deleteResult = await controller.SoftDeleteInventory(created.Id);
        Assert.IsType<NoContentResult>(deleteResult);

        var getResult = await controller.GetInventory(created.Id);
        Assert.IsType<NotFoundResult>(getResult.Result);
    }

    [Fact]
    public async Task GetAll_Returns_Only_NotDeleted_Inventory()
    {
        var context = GetInMemoryDbContext();
        context.Products.Add(new Product { Id = 1 });
        context.Warehouses.Add(new Warehouse { Id = 1 });
        await context.SaveChangesAsync();

        var controller = GetController(context);

        await controller.CreateInventory(CreateTestInventory());

        var toDelete = CreateTestInventory();
        var create = await controller.CreateInventory(toDelete);
        var created = (create.Result as CreatedAtActionResult)?.Value as Inventory;
        await controller.SoftDeleteInventory(created.Id);

        var getAll = await controller.GetAllInventories();
        var okResult = getAll.Result as OkObjectResult;
        var inventories = okResult?.Value as List<Inventory>;

        Assert.Single(inventories);
        Assert.DoesNotContain(inventories, i => i.Id == created.Id);
    }
}
