using gentech_services.Models;

namespace gentech_services.Repositories
{
    public interface IInventoryLogRepository
    {
        Task<IEnumerable<InventoryLog>> GetAllAsync();
        Task<IEnumerable<InventoryLog>> GetByProductIdAsync(int productId);
        Task<InventoryLog?> GetByIdAsync(int id);
        Task<InventoryLog> AddAsync(InventoryLog inventoryLog);
        Task UpdateAsync(InventoryLog inventoryLog);
        Task DeleteAsync(int id);
    }
}
