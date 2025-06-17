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
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    private OrderController GetOrderController(ApplicationDbContext context)
    {
        var service = new OrderService(context);
        return new OrderController(service);
    }

    private async Task<Customer> CreateTestCustomer(ApplicationDbContext context)
    {
        var customer = new Customer
        {
            BedrijfsNaam = "Test BV",
            ContactPersoon = "Jan Jansen",
            Email = "test@example.com",
            TelefoonNummer = "0612345678",
            Adres = "Straat 1, Amsterdam"
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        return customer;
    }

    private async Task<Product> CreateTestProduct(ApplicationDbContext context)
    {
        var product = new Product
        {
            ProductName = "Test Product",
            SKU = "TP001",
            WeightKg = 5,
            Material = "Plastic",
            BatchNumber = 123,
            Price = 10.0,
            Category = "Tools"
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    private static string ToDateString(DateTime dt) => dt.ToString("dd-MM-yyyy");
    private static string ToTimeString(DateTime dt) => dt.ToString("HH:mm");

    [Fact]
    public async Task Can_Create_And_Get_Order()
    {
        var context = GetInMemoryDbContext();
        var controller = GetOrderController(context);

        var customer = await CreateTestCustomer(context);
        var product = await CreateTestProduct(context);

        var orderDto = new OrderCreateDto
        {
            CustomerId = customer.Id,
            OrderDate = ToDateString(DateTime.Now),
            OrderTime = ToTimeString(DateTime.Now),
            DeliveryAddress = "Teststraat 123",
            ExpectedDeliveryDate = ToDateString(DateTime.Now.AddDays(3)),
            ExpectedDeliveryTime = ToTimeString(DateTime.Now.AddDays(3)),
            Status = OrderStatus.Pending,
            ProductLines = new List<OrderLineCreateDto>
            {
                new OrderLineCreateDto { ProductId = product.Id, Quantity = 2 }
            }
        };

        var createResult = await controller.CreateOrder(orderDto);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Order;

        Assert.NotNull(created);
        Assert.Equal(customer.Id, created.CustomerId);

        var getResult = await controller.GetOrderById(created.Id);
        var fetched = (getResult as OkObjectResult)?.Value;

        Assert.NotNull(fetched);
    }

    [Fact]
    public async Task Can_Update_Order()
    {
        var context = GetInMemoryDbContext();
        var controller = GetOrderController(context);

        var customer = await CreateTestCustomer(context);
        var product = await CreateTestProduct(context);

        var orderDto = new OrderCreateDto
        {
            CustomerId = customer.Id,
            OrderDate = ToDateString(DateTime.Now),
            OrderTime = ToTimeString(DateTime.Now),
            DeliveryAddress = "Original Address",
            ExpectedDeliveryDate = ToDateString(DateTime.Now.AddDays(5)),
            ExpectedDeliveryTime = ToTimeString(DateTime.Now.AddDays(5)),
            Status = OrderStatus.Pending,
            ProductLines = new List<OrderLineCreateDto>
            {
                new OrderLineCreateDto { ProductId = product.Id, Quantity = 1 }
            }
        };

        var createResult = await controller.CreateOrder(orderDto);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Order;

        created.DeliveryAddress = "Updated Address";

        var updateResult = await controller.UpdateOrder(created.Id, created);
        var updated = (updateResult.Result as OkObjectResult)?.Value as Order;

        Assert.NotNull(updated);
        Assert.Equal("Updated Address", updated.DeliveryAddress);
    }

    [Fact]
    public async Task Can_SoftDelete_Order()
    {
        var context = GetInMemoryDbContext();
        var controller = GetOrderController(context);

        var customer = await CreateTestCustomer(context);
        var product = await CreateTestProduct(context);

        var orderDto = new OrderCreateDto
        {
            CustomerId = customer.Id,
            OrderDate = ToDateString(DateTime.Now),
            OrderTime = ToTimeString(DateTime.Now),
            DeliveryAddress = "SoftDelete Address",
            ExpectedDeliveryDate = ToDateString(DateTime.Now.AddDays(2)),
            ExpectedDeliveryTime = ToTimeString(DateTime.Now.AddDays(2)),
            Status = OrderStatus.Pending,
            ProductLines = new List<OrderLineCreateDto>
            {
                new OrderLineCreateDto { ProductId = product.Id, Quantity = 1 }
            }
        };

        var createResult = await controller.CreateOrder(orderDto);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Order;

        var deleteResult = await controller.SoftDeleteOrder(created.Id);
        Assert.IsType<NoContentResult>(deleteResult);

        var getResult = await controller.GetOrderById(created.Id);
        Assert.IsType<NotFoundResult>(getResult);
    }

    [Fact]
    public async Task GetAllOrders_Returns_Only_Active()
    {
        var context = GetInMemoryDbContext();
        var controller = GetOrderController(context);

        var customer = await CreateTestCustomer(context);
        var product = await CreateTestProduct(context);

        var activeOrderDto = new OrderCreateDto
        {
            CustomerId = customer.Id,
            OrderDate = ToDateString(DateTime.Now),
            OrderTime = ToTimeString(DateTime.Now),
            DeliveryAddress = "Active Order",
            ExpectedDeliveryDate = ToDateString(DateTime.Now.AddDays(3)),
            ExpectedDeliveryTime = ToTimeString(DateTime.Now.AddDays(3)),
            Status = OrderStatus.Pending,
            ProductLines = new List<OrderLineCreateDto>
            {
                new OrderLineCreateDto { ProductId = product.Id, Quantity = 1 }
            }
        };
        await controller.CreateOrder(activeOrderDto);

        var deletedOrderDto = new OrderCreateDto
        {
            CustomerId = customer.Id,
            OrderDate = ToDateString(DateTime.Now),
            OrderTime = ToTimeString(DateTime.Now),
            DeliveryAddress = "Deleted Order",
            ExpectedDeliveryDate = ToDateString(DateTime.Now.AddDays(3)),
            ExpectedDeliveryTime = ToTimeString(DateTime.Now.AddDays(3)),
            Status = OrderStatus.Pending,
            ProductLines = new List<OrderLineCreateDto>
            {
                new OrderLineCreateDto { ProductId = product.Id, Quantity = 1 }
            }
        };

        var createdResult = await controller.CreateOrder(deletedOrderDto);
        var createdOrder = (createdResult.Result as CreatedAtActionResult)?.Value as Order;

        await controller.SoftDeleteOrder(createdOrder.Id);

        var getAllResult = await controller.GetAllOrders();
        var orders = (getAllResult as OkObjectResult)?.Value as IEnumerable<object>;

        Assert.Single(orders);
    }
}
