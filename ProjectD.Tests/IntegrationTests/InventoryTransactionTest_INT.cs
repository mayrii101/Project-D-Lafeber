using Xunit;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
using AzureSqlConnectionDemo.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class InventoryTransactionIntegrationTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    private InventoryTransactionController GetController(ApplicationDbContext context)
    {
        var service = new InventoryTransactionService(context);
        return new InventoryTransactionController(service);
    }

    private InventoryTransaction CreateTestTransaction()
    {
        return new InventoryTransaction
        {
            ProductId = 1,
            Quantity = 10,
            Type = InventoryTransactionType.Inbound,
            Timestamp = DateTime.UtcNow,
            EmployeeId = 1,
            SourceOrDestination = "Magazijn A"
        };
    }

    [Fact]
    public async Task Can_Create_And_Get_InventoryTransaction()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var transaction = CreateTestTransaction();

        var createResult = await controller.CreateInventoryTransaction(transaction);
        var createdResult = createResult.Result as CreatedAtActionResult;
        var created = createdResult?.Value as InventoryTransaction;

        var getResult = await controller.GetInventoryTransactionById(created!.Id);
        var fetchedResult = getResult.Result as OkObjectResult;
        var fetched = fetchedResult?.Value as InventoryTransaction;

        Assert.NotNull(created);
        Assert.NotNull(fetched);
        Assert.Equal(transaction.Quantity, fetched.Quantity);
        Assert.Equal(transaction.SourceOrDestination, fetched.SourceOrDestination);
    }

    [Fact]
    public async Task Can_Update_InventoryTransaction()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var transaction = CreateTestTransaction();

        var createResult = await controller.CreateInventoryTransaction(transaction);
        var createdResult = createResult.Result as CreatedAtActionResult;
        var created = createdResult?.Value as InventoryTransaction;

        created!.Quantity = 20;

        var updateResult = await controller.UpdateInventoryTransaction(created.Id, created);
        var okResult = updateResult.Result as OkObjectResult;
        var updated = okResult?.Value as InventoryTransaction;

        Assert.NotNull(updated);
        Assert.Equal(20, updated.Quantity);
    }

    [Fact]
    public async Task Can_SoftDelete_InventoryTransaction()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var transaction = CreateTestTransaction();

        var createResult = await controller.CreateInventoryTransaction(transaction);
        var createdResult = createResult.Result as CreatedAtActionResult;
        var created = createdResult?.Value as InventoryTransaction;

        var deleteResult = await controller.SoftDeleteInventoryTransaction(created!.Id);
        Assert.IsType<NoContentResult>(deleteResult);

        var getResult = await controller.GetInventoryTransactionById(created.Id);
        Assert.IsType<NotFoundResult>(getResult.Result);
    }

    [Fact]
    public async Task GetAll_Returns_Only_NotDeleted_Transactions()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        await controller.CreateInventoryTransaction(CreateTestTransaction());

        var deletedTransaction = CreateTestTransaction();
        var createResult = await controller.CreateInventoryTransaction(deletedTransaction);
        var createdResult = createResult.Result as CreatedAtActionResult;
        var created = createdResult?.Value as InventoryTransaction;

        await controller.SoftDeleteInventoryTransaction(created!.Id);

        var getAllResult = await controller.GetInventoryTransactions();
        var okResult = getAllResult.Result as OkObjectResult;
        var transactions = okResult?.Value as List<InventoryTransaction>;

        Assert.Single(transactions);
        Assert.DoesNotContain(transactions, t => t.Id == created.Id);
    }
}
