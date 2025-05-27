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

        var createActionResult = await controller.CreateProduct(product);
        var createdResult = createActionResult.Result as CreatedAtActionResult;
        var created = createdResult?.Value as Product;

        var getActionResult = await controller.GetProduct(created.Id);
        var fetchedResult = getActionResult.Result as OkObjectResult;
        var fetchedProduct = fetchedResult?.Value as Product;

        Assert.NotNull(created);
        Assert.NotNull(fetchedProduct);
        Assert.Equal(product.ProductName, fetchedProduct.ProductName);
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
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Product;

        created.ProductName = "Nieuwe Naam";
        var updateResult = await controller.UpdateProduct(created.Id, created);
        var updated = (updateResult.Result as OkObjectResult)?.Value as Product;

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
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Product;

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

        await controller.CreateProduct(new Product { ProductName = "Actief Product", SKU = "A1", Price = 5.0 });
        var toDelete = new Product { ProductName = "Verwijderd Product", SKU = "B1", Price = 7.0 };
        var result = await controller.CreateProduct(toDelete);
        var created = (result.Result as CreatedAtActionResult)?.Value as Product;
        await controller.SoftDeleteProduct(created.Id);

        var getAllResult = await controller.GetAllProducts();
        var okResult = getAllResult.Result as OkObjectResult;
        var products = okResult?.Value as List<Product>;

        Assert.Single(products);
        Assert.DoesNotContain(products, p => p.ProductName == "Verwijderd Product");
    }
}