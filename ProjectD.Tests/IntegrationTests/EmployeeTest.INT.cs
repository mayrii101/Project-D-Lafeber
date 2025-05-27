using Xunit;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
using AzureSqlConnectionDemo.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

public class EmployeeIntegrationTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    private EmployeeController GetController(ApplicationDbContext context)
    {
        var service = new EmployeeService(context);
        return new EmployeeController(service);
    }

    [Fact]
    public async Task Can_Create_And_Get_Employee()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var employee = new Employee
        {
            Name = "Lisa de Jong",
            Role = "Magazijnmedewerker",
            Email = "lisa@example.com"
        };

        // Act
        var createResult = await controller.CreateEmployee(employee);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Employee;

        var getResult = await controller.GetEmployee(created.Id);
        var fetched = (getResult.Result as OkObjectResult)?.Value as Employee;

        // Assert
        Assert.NotNull(created);
        Assert.NotNull(fetched);
        Assert.Equal("Lisa de Jong", fetched.Name);
    }

    [Fact]
    public async Task Can_Update_Employee()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var employee = new Employee { Name = "Pieter", Role = "Bezorger", Email = "pieter@bedrijf.nl" };
        var createResult = await controller.CreateEmployee(employee);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Employee;

        created.Name = "Pieter de Groot";
        var updateResult = await controller.UpdateEmployee(created.Id, created);
        var updated = (updateResult.Result as OkObjectResult)?.Value as Employee;

        Assert.Equal("Pieter de Groot", updated.Name);
    }

    [Fact]
    public async Task Can_SoftDelete_Employee()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        var employee = new Employee { Name = "Anna", Role = "Planner", Email = "anna@bedrijf.nl" };
        var createResult = await controller.CreateEmployee(employee);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Employee;

        var deleteResult = await controller.SoftDeleteEmployee(created.Id);
        Assert.IsType<NoContentResult>(deleteResult);

        var getResult = await controller.GetEmployee(created.Id);
        Assert.IsType<NotFoundResult>(getResult.Result);
    }

    [Fact]
    public async Task GetAll_Returns_Only_NotDeleted_Employees()
    {
        var context = GetInMemoryDbContext();
        var controller = GetController(context);

        await controller.CreateEmployee(new Employee { Name = "Actieve Werknemer", Role = "Technicus", Email = "a@a.nl" });

        var deletedEmployee = new Employee { Name = "Verwijderd", Role = "Tester", Email = "b@b.nl" };
        var createResult = await controller.CreateEmployee(deletedEmployee);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as Employee;
        await controller.SoftDeleteEmployee(created.Id);

        var getAllResult = await controller.GetAllEmployees();
        var result = (getAllResult.Result as OkObjectResult)?.Value as List<Employee>;

        Assert.Single(result);
        Assert.DoesNotContain(result, e => e.Name == "Verwijderd");
    }
}