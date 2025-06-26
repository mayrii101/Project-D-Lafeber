using Microsoft.EntityFrameworkCore;
using ProjectD.Models;

namespace ProjectD.Services
{
    public interface IInventoryService
    {
        Task<List<Inventory>> GetAllInventoriesAsync();
        Task<Inventory> GetInventoryByIdAsync(int id);
        Task<Inventory> CreateInventoryAsync(Inventory inventory);
        Task<Inventory> UpdateInventoryAsync(int id, Inventory inventory);
        Task<bool> SoftDeleteInventoryAsync(int id);
    }

    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Inventory>> GetAllInventoriesAsync()
        {
            return await _context.Inventories
                                 .Where(i => !i.IsDeleted)
                                 .ToListAsync();
        }

        public async Task<Inventory> GetInventoryByIdAsync(int id)
        {
            return await _context.Inventories
                                 .Where(i => i.Id == id && !i.IsDeleted)
                                 .FirstOrDefaultAsync();
        }

        public async Task<Inventory> CreateInventoryAsync(Inventory inventory)
        {
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<Inventory> UpdateInventoryAsync(int id, Inventory inventory)
        {
            var existingInventory = await _context.Inventories.FindAsync(id);
            if (existingInventory == null) return null;

            existingInventory.WarehouseId = inventory.WarehouseId;
            existingInventory.ProductId = inventory.ProductId;
            existingInventory.QuantityOnHand = inventory.QuantityOnHand;

            await _context.SaveChangesAsync();
            return existingInventory;
        }

        public async Task<bool> SoftDeleteInventoryAsync(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null || inventory.IsDeleted) return false;

            inventory.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}