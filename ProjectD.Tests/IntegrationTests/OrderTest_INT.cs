using Xunit;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
using AzureSqlConnectionDemo.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OrderIntegrationTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"OrderDb_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    private OrderController GetController(ApplicationDbContext context)
    {
        var service = new OrderService(context);
        return new OrderController(service);
    }

    private Customer SeedCustomer(ApplicationDbContext context)
    {
        var customer = new Customer { BedrijfsNaam = "Test Customer", Email = "test@example.com" };
        context.Customers.Add(customer);
        context.SaveChanges();
        return customer;
    }

    [Fact]
    public async Task Can_Create_And_Get_Order()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);
        var customer = SeedCustomer(context);

        var order = new Order
        {
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow,
            DeliveryAddress = "123 Test Street",
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(7),
            Status = OrderStatus.Pending
        };

        var createResult = await controller.CreateOrder(order);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Order;

        var getResult = await controller.GetOrder(created.Id);
        var fetched = (getResult.Result as OkObjectResult)?.Value as Order;

        Assert.NotNull(created);
        Assert.NotNull(fetched);
        Assert.Equal(order.DeliveryAddress, fetched.DeliveryAddress);
    }

    [Fact]
    public async Task Can_Update_Order()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);
        var customer = SeedCustomer(context);

        var order = new Order
        {
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow,
            DeliveryAddress = "Old Address",
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(3),
            Status = OrderStatus.Pending
        };

        var createResult = await controller.CreateOrder(order);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Order;

        created.DeliveryAddress = "Updated Address";
        var updateResult = await controller.UpdateOrder(created.Id, created);
        var updated = (updateResult.Result as OkObjectResult)?.Value as Order;

        Assert.Equal("Updated Address", updated.DeliveryAddress);
    }

    [Fact]
    public async Task Can_SoftDelete_Order()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);
        var customer = SeedCustomer(context);

        var order = new Order
        {
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow,
            DeliveryAddress = "To be deleted",
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(3),
            Status = OrderStatus.Pending
        };

        var createResult = await controller.CreateOrder(order);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Order;

        var deleteResult = await controller.SoftDeleteOrder(created.Id);
        Assert.IsType<NoContentResult>(deleteResult);

        var getResult = await controller.GetOrder(created.Id);
        Assert.IsType<NotFoundResult>(getResult.Result);
    }

    [Fact]
    public async Task GetAll_Returns_Only_NotDeleted_Orders()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);
        var customer = SeedCustomer(context);

        await controller.CreateOrder(new Order
        {
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow,
            DeliveryAddress = "Active Order",
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(1),
            Status = OrderStatus.Pending
        });

        var toDelete = new Order
        {
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow,
            DeliveryAddress = "Deleted Order",
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(1),
            Status = OrderStatus.Pending
        };

        var createResult = await controller.CreateOrder(toDelete);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Order;

        await controller.SoftDeleteOrder(created.Id);

        var getAllResult = await controller.GetAllOrders();
        var okResult = getAllResult.Result as OkObjectResult;
        var orders = okResult?.Value as List<Order>;

        Assert.Single(orders);
        Assert.DoesNotContain(orders, o => o.DeliveryAddress == "Deleted Order");
    }
}
