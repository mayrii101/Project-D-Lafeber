using AzureSqlConnectionDemo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureSqlConnectionDemo.Services
{
    public interface IInventoryTransactionService
    {
        Task<List<InventoryTransaction>> GetAllInventoryTransactionsAsync();
        Task<InventoryTransaction> GetInventoryTransactionByIdAsync(int id);
        Task<InventoryTransaction> CreateInventoryTransactionAsync(InventoryTransaction transaction);
        Task<InventoryTransaction> UpdateInventoryTransactionAsync(int id, InventoryTransaction transaction);
        Task<bool> SoftDeleteInventoryTransactionAsync(int id);  // Soft delete instead of hard delete
    }
    public class InventoryTransactionService : IInventoryTransactionService
    {
        private readonly ApplicationDbContext _context;

        public InventoryTransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventoryTransaction>> GetAllInventoryTransactionsAsync()
        {
            return await _context.InventoryTransactions.Where(t => !t.IsDeleted).ToListAsync();
        }

        public async Task<InventoryTransaction> GetInventoryTransactionByIdAsync(int id)
        {
            return await _context.InventoryTransactions
                .Where(t => !t.IsDeleted)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<InventoryTransaction> CreateInventoryTransactionAsync(InventoryTransaction transaction)
        {
            // Add the new transaction and save to the database
            _context.InventoryTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<InventoryTransaction> UpdateInventoryTransactionAsync(int id, InventoryTransaction transaction)
        {
            // Find the existing inventory transaction
            var existingTransaction = await _context.InventoryTransactions.FindAsync(id);
            if (existingTransaction == null || existingTransaction.IsDeleted) return null;

            // Update the transaction properties
            existingTransaction.Quantity = transaction.Quantity;
            existingTransaction.Type = transaction.Type;
            existingTransaction.Timestamp = transaction.Timestamp;
            existingTransaction.SourceOrDestination = transaction.SourceOrDestination;
            existingTransaction.EmployeeId = transaction.EmployeeId;

            // Save the changes to the database
            await _context.SaveChangesAsync();
            return existingTransaction;
        }

        public async Task<bool> SoftDeleteInventoryTransactionAsync(int id)
        {
            // Find the inventory transaction to soft delete
            var transaction = await _context.InventoryTransactions.FindAsync(id);
            if (transaction == null || transaction.IsDeleted) return false;

            // Mark as deleted (soft delete)
            transaction.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}