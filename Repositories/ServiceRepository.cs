using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gentech_services.Data;
using gentech_services.Models;

namespace gentech_services.Repositories
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        public ServiceRepository(GentechDbContext context) : base(context) { }

        public async Task<IEnumerable<Service>> GetByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Include(s => s.Category)
                .Where(s => s.CategoryID == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetActiveServicesAsync()
        {
            return await _dbSet
                .Include(s => s.Category)
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Service?> GetByIdWithCategoryAsync(int serviceId)
        {
            return await _dbSet
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.ServiceID == serviceId);
        }

        public async Task<IEnumerable<Service>> SearchByNameAsync(string searchTerm)
        {
            return await _dbSet
                .Include(s => s.Category)
                .Where(s => s.Name.Contains(searchTerm) && s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public override async Task<Service?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.ServiceID == id);
        }

        public override async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _dbSet
                .Include(s => s.Category)
                .OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
                .ToListAsync();
        }
    }
}
