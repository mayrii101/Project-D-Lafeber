using Xunit;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
using AzureSqlConnectionDemo.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProductIntegrationTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    private ProductController GetController(ApplicationDbContext context)
    {
        var service = new ProductService(context);
        return new ProductController(service);
    }

    [Fact]
    public async Task Can_Create_And_Get_Product()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var product = new Product
        {
            ProductName = "Test Product",
            SKU = "TP123",
            WeightKg = 2,
            Material = "Plastic",
            BatchNumber = 1001,
            Price = 19.99,
            Category = "Test"
        };

        var createResult = await controller.CreateProduct(product);
        var createdResult = createResult as CreatedAtActionResult;
        var createdProduct = createdResult?.Value as Product;

        Assert.NotNull(createdProduct);

        var getResult = await controller.GetProduct(createdProduct.Id);
        var fetchedResult = getResult.Result as OkObjectResult;
        var fetchedProduct = fetchedResult?.Value as Product;

        Assert.NotNull(fetchedProduct);
        Assert.Equal("Test Product", fetchedProduct.ProductName);
        Assert.Equal("TP123", fetchedProduct.SKU);
    }

    [Fact]
    public async Task Can_Update_Product()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var product = new Product
        {
            ProductName = "Oude Naam",
            SKU = "SKU001",
            WeightKg = 5,
            Material = "Staal",
            BatchNumber = 200,
            Price = 45.00,
            Category = "Metaal"
        };

        var createResult = await controller.CreateProduct(product);
        var created = (createResult as CreatedAtActionResult)?.Value as Product;

        created.ProductName = "Nieuwe Naam";

        var updateResult = await controller.UpdateProduct(created.Id, created);
        var okResult = updateResult.Result as OkObjectResult;
        var updated = okResult?.Value as Product;

        Assert.NotNull(updated);
        Assert.Equal("Nieuwe Naam", updated.ProductName);
    }

    [Fact]
    public async Task Can_SoftDelete_Product()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var product = new Product
        {
            ProductName = "Verwijder Mij",
            SKU = "DEL123",
            WeightKg = 1,
            Material = "Hout",
            BatchNumber = 999,
            Price = 10.00,
            Category = "Tijdelijk"
        };

        var createResult = await controller.CreateProduct(product);
        var created = (createResult as CreatedAtActionResult)?.Value as Product;

        var deleteResult = await controller.SoftDeleteProduct(created.Id);
        Assert.IsType<NoContentResult>(deleteResult);

        var getResult = await controller.GetProduct(created.Id);
        Assert.IsType<NotFoundResult>(getResult.Result);
    }

    [Fact]
    public async Task GetAll_Returns_Only_NotDeleted_Products()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        await controller.CreateProduct(new Product
        {
            ProductName = "Actief Product",
            SKU = "A1",
            Price = 5.0
        });

        var toDelete = new Product
        {
            ProductName = "Verwijderd Product",
            SKU = "B1",
            Price = 7.0
        };

        var result = await controller.CreateProduct(toDelete);
        var created = (result as CreatedAtActionResult)?.Value as Product;

        await controller.SoftDeleteProduct(created.Id);

        var getAllResult = await controller.GetAllProducts();
        var okResult = getAllResult.Result as OkObjectResult;
        var products = okResult?.Value as List<Product>;

        Assert.NotNull(products);
        Assert.Single(products);
        Assert.DoesNotContain(products, p => p.ProductName == "Verwijderd Product");
    }
}