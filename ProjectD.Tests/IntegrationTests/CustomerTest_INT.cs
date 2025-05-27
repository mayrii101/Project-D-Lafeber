using Xunit;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
using AzureSqlConnectionDemo.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

public class CustomerIntegrationTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    private CustomerController GetController(ApplicationDbContext context)
    {
        var service = new CustomerService(context);
        return new CustomerController(service);
    }

    [Fact]
    public async Task Can_Create_And_Get_Customer()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var customer = new Customer
        {
            BedrijfsNaam = "Test BV",
            ContactPersoon = "Jan Jansen",
            Email = "test@example.com",
            TelefoonNummer = "0612345678",
            Adres = "Straat 1, Amsterdam"
        };

        // Act
        var createActionResult = await controller.CreateCustomer(customer);
        var createdResult = createActionResult.Result as CreatedAtActionResult;
        var created = createdResult?.Value as Customer;

        var getActionResult = await controller.GetCustomer(created.Id);
        var fetchedResult = getActionResult.Result as OkObjectResult;
        var fetchedCustomer = fetchedResult?.Value as Customer;

        // Assert
        Assert.NotNull(created);
        Assert.NotNull(fetchedCustomer);
        Assert.Equal(customer.BedrijfsNaam, fetchedCustomer.BedrijfsNaam);
    }

    [Fact]
    public async Task Can_Update_Customer()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var customer = new Customer
        {
            BedrijfsNaam = "Oud BV",
            ContactPersoon = "Piet",
            Email = "piet@oud.nl",
            TelefoonNummer = "0600000000",
            Adres = "Oude Straat 2"
        };

        var createActionResult = await controller.CreateCustomer(customer);
        var createdResult = createActionResult.Result as CreatedAtActionResult;
        var created = createdResult?.Value as Customer;

        created.BedrijfsNaam = "Nieuw BV";
        var updateActionResult = await controller.UpdateCustomer(created.Id, created);
        var updateResult = updateActionResult.Result as OkObjectResult;
        var updated = updateResult?.Value as Customer;

        Assert.Equal("Nieuw BV", updated.BedrijfsNaam);
    }

    [Fact]
    public async Task Can_SoftDelete_Customer()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var customer = new Customer
        {
            BedrijfsNaam = "Delete BV",
            ContactPersoon = "Kees",
            Email = "kees@delete.nl",
            TelefoonNummer = "0622222222",
            Adres = "Vergeetstraat 3"
        };

        var createActionResult = await controller.CreateCustomer(customer);
        var createdResult = createActionResult.Result as CreatedAtActionResult;
        var created = createdResult?.Value as Customer;

        var deleteResult = await controller.SoftDeleteCustomer(created.Id);
        Assert.IsType<NoContentResult>(deleteResult);

        var getActionResult = await controller.GetCustomer(created.Id);
        Assert.IsType<NotFoundResult>(getActionResult.Result);
    }

    [Fact]
    public async Task GetAll_Returns_Only_NotDeleted_Customers()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        await controller.CreateCustomer(new Customer { BedrijfsNaam = "Actief", ContactPersoon = "A", Email = "a@a.nl" });
        var deletedCustomer = new Customer { BedrijfsNaam = "Verwijderd", ContactPersoon = "B", Email = "b@b.nl" };
        var createActionResult = await controller.CreateCustomer(deletedCustomer);
        var createdResult = createActionResult.Result as CreatedAtActionResult;
        var created = createdResult?.Value as Customer;
        await controller.SoftDeleteCustomer(created.Id);

        var getAllActionResult = await controller.GetAllCustomers();
        var okResult = getAllActionResult.Result as OkObjectResult;
        var customers = okResult?.Value as List<Customer>;

        Assert.Single(customers); // Alleen 'Actief' over
        Assert.DoesNotContain(customers, c => c.BedrijfsNaam == "Verwijderd");
    }
}