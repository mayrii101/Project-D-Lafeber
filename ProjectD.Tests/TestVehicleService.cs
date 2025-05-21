using Xunit;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
using System.Threading.Tasks;

public class VehicleServiceTests
{
    [Fact]
    public async Task UpdateVehicleAsync_UpdatesVehicleSuccessfully()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "UpdateVehicleDb")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            var vehicle = new Vehicle
            {
                Id = 1,
                LicensePlate = "ABC-123",
                CapacityKg = 10000,
                Type = VehicleType.Kipper,
                Status = VehicleStatus.Available,
                IsDeleted = false
            };

            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var service = new VehicleService(context);

            var updatedVehicle = new Vehicle
            {
                Type = VehicleType.LowbedTrailer,
                Status = VehicleStatus.Maintenance
            };

            var result = await service.UpdateVehicleAsync(1, updatedVehicle);

            Assert.NotNull(result);
            Assert.Equal(VehicleType.LowbedTrailer, result.Type);
            Assert.Equal(VehicleStatus.Maintenance, result.Status);
        }
    }
}
