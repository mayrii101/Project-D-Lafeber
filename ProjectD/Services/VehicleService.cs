using Microsoft.EntityFrameworkCore;
using ProjectD.Models;

namespace ProjectD.Services
{
    public interface IVehicleService
    {
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle> GetVehicleByIdAsync(int id);
        Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
        Task<Vehicle> UpdateVehicleAsync(int id, Vehicle vehicle);
        Task<bool> SoftDeleteVehicleAsync(int id);
    }

    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext _context;

        public VehicleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            return await _context.Vehicles.Where(v => !v.IsDeleted).ToListAsync();
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles.Where(v => !v.IsDeleted).FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<Vehicle> UpdateVehicleAsync(int id, Vehicle vehicle)
        {
            var existingVehicle = await _context.Vehicles.FindAsync(id);
            if (existingVehicle == null || existingVehicle.IsDeleted) return null;

            existingVehicle.Type = vehicle.Type;
            existingVehicle.Status = vehicle.Status;

            await _context.SaveChangesAsync();
            return existingVehicle;
        }

        public async Task<bool> SoftDeleteVehicleAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null || vehicle.IsDeleted) return false;

            vehicle.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}