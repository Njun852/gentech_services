using gentech_services.Data;
using gentech_services.Models;
using Microsoft.EntityFrameworkCore;

namespace gentech_services.Repositories
{
    public class InventoryLogRepository : IInventoryLogRepository
    {
        private readonly GentechDbContext _context;

        public InventoryLogRepository(GentechDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryLog>> GetAllAsync()
        {
            return await _context.InventoryLogs
                .Include(il => il.Product)
                .Include(il => il.User)
                .OrderByDescending(il => il.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryLog>> GetByProductIdAsync(int productId)
        {
            return await _context.InventoryLogs
                .Include(il => il.Product)
                .Include(il => il.User)
                .Where(il => il.ProductID == productId)
                .OrderByDescending(il => il.CreatedAt)
                .ToListAsync();
        }

        public async Task<InventoryLog?> GetByIdAsync(int id)
        {
            return await _context.InventoryLogs
                .Include(il => il.Product)
                .Include(il => il.User)
                .FirstOrDefaultAsync(il => il.InventoryLogID == id);
        }

        public async Task<InventoryLog> AddAsync(InventoryLog inventoryLog)
        {
            _context.InventoryLogs.Add(inventoryLog);
            await _context.SaveChangesAsync();
            return inventoryLog;
        }

        public async Task UpdateAsync(InventoryLog inventoryLog)
        {
            _context.InventoryLogs.Update(inventoryLog);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var inventoryLog = await GetByIdAsync(id);
            if (inventoryLog != null)
            {
                _context.InventoryLogs.Remove(inventoryLog);
                await _context.SaveChangesAsync();
            }
        }
    }
}
