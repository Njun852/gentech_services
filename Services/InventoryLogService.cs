using gentech_services.Models;
using gentech_services.Repositories;

namespace gentech_services.Services
{
    public class InventoryLogService
    {
        private readonly IInventoryLogRepository _inventoryLogRepository;

        public InventoryLogService(IInventoryLogRepository inventoryLogRepository)
        {
            _inventoryLogRepository = inventoryLogRepository;
        }

        public async Task<IEnumerable<InventoryLog>> GetAllLogsAsync()
        {
            return await _inventoryLogRepository.GetAllAsync();
        }

        public async Task<IEnumerable<InventoryLog>> GetProductLogsAsync(int productId)
        {
            return await _inventoryLogRepository.GetByProductIdAsync(productId);
        }

        public async Task<InventoryLog?> GetLogByIdAsync(int id)
        {
            return await _inventoryLogRepository.GetByIdAsync(id);
        }

        public async Task<InventoryLog> CreateLogAsync(
            int productId,
            string changeType,
            int quantityChanged,
            int previousQuantity,
            int newQuantity,
            int createdBy,
            string? reason = null)
        {
            var inventoryLog = new InventoryLog
            {
                ProductID = productId,
                ChangeType = changeType,
                QuantityChanged = quantityChanged,
                PreviousQuantity = previousQuantity,
                NewQuantity = newQuantity,
                CreatedBy = createdBy,
                Reason = reason,
                CreatedAt = DateTime.Now
            };

            return await _inventoryLogRepository.AddAsync(inventoryLog);
        }
    }
}
