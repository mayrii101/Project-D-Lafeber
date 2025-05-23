using Microsoft.EntityFrameworkCore;
using ProjectD.Models;

namespace ProjectD.Services
{
    public interface IWarehouseService
    {
        Task<List<Warehouse>> GetAllWarehousesAsync();
        Task<Warehouse> GetWarehouseByIdAsync(int id);
        Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse);
        Task<Warehouse> UpdateWarehouseAsync(int id, Warehouse warehouse);
        Task<bool> SoftDeleteWarehouseAsync(int id);
    }

    public class WarehouseService : IWarehouseService
    {
        private readonly ApplicationDbContext _context;

        public WarehouseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Warehouse>> GetAllWarehousesAsync()
        {
            return await _context.Warehouses.Where(w => !w.IsDeleted).ToListAsync();
        }

        public async Task<Warehouse> GetWarehouseByIdAsync(int id)
        {
            return await _context.Warehouses.Where(w => !w.IsDeleted).FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse)
        {
            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();
            return warehouse;
        }

        public async Task<Warehouse> UpdateWarehouseAsync(int id, Warehouse warehouse)
        {
            var existingWarehouse = await _context.Warehouses.FindAsync(id);
            if (existingWarehouse == null || existingWarehouse.IsDeleted) return null;

            existingWarehouse.Name = warehouse.Name;
            existingWarehouse.Location = warehouse.Location;

            await _context.SaveChangesAsync();
            return existingWarehouse;
        }

        public async Task<bool> SoftDeleteWarehouseAsync(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null || warehouse.IsDeleted) return false;

            warehouse.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}