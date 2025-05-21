using Xunit;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
using System.Threading.Tasks;

public class WarehouseServiceTests
{
    [Fact]
    public async Task UpdateWarehouseAsync_UpdatesWarehouseSuccessfully()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "UpdateWarehouseDb")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            var warehouse = new Warehouse
            {
                Id = 1,
                Name = "Main Warehouse",
                Location = "Amsterdam",
                ContactPerson = "Jan Jansen",
                Phone = "0101234567",
                IsDeleted = false
            };

            context.Warehouses.Add(warehouse);
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var service = new WarehouseService(context);

            var updatedWarehouse = new Warehouse
            {
                Name = "Updated Warehouse",
                Location = "Rotterdam"
            };

            var result = await service.UpdateWarehouseAsync(1, updatedWarehouse);

            Assert.NotNull(result);
            Assert.Equal("Updated Warehouse", result.Name);
            Assert.Equal("Rotterdam", result.Location);
        }
    }
}
