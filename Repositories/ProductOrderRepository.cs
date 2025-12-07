using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gentech_services.Data;
using gentech_services.Models;

namespace gentech_services.Repositories
{
    public class ProductOrderRepository : Repository<ProductOrder>, IProductOrderRepository
    {
        public ProductOrderRepository(GentechDbContext context) : base(context) { }

        public async Task<IEnumerable<ProductOrder>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Include(po => po.ProductOrderItems)
                    .ThenInclude(poi => poi.Product)
                        .ThenInclude(p => p.Category)
                .Where(po => po.Status == status)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(po => po.ProductOrderItems)
                    .ThenInclude(poi => poi.Product)
                        .ThenInclude(p => p.Category)
                .Where(po => po.CreatedAt >= startDate && po.CreatedAt <= endDate)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductOrder>> GetByCustomerAsync(string customerEmail)
        {
            return await _dbSet
                .Include(po => po.ProductOrderItems)
                    .ThenInclude(poi => poi.Product)
                        .ThenInclude(p => p.Category)
                .Where(po => po.Email == customerEmail)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();
        }

        public async Task<ProductOrder?> GetByIdWithDetailsAsync(int productOrderId)
        {
            return await _dbSet
                .Include(po => po.ProductOrderItems)
                    .ThenInclude(poi => poi.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(po => po.ProductOrderID == productOrderId);
        }

        public async Task<IEnumerable<ProductOrder>> GetRecentOrdersAsync(int count = 10)
        {
            return await _dbSet
                .Include(po => po.ProductOrderItems)
                    .ThenInclude(poi => poi.Product)
                        .ThenInclude(p => p.Category)
                .OrderByDescending(po => po.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public override async Task<ProductOrder?> GetByIdAsync(int id)
        {
            return await GetByIdWithDetailsAsync(id);
        }

        public override async Task<IEnumerable<ProductOrder>> GetAllAsync()
        {
            return await _dbSet
                .Include(po => po.ProductOrderItems)
                    .ThenInclude(poi => poi.Product)
                        .ThenInclude(p => p.Category)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();
        }
    }
}
