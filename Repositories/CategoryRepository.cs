using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gentech_services.Data;
using gentech_services.Models;

namespace gentech_services.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(GentechDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetByTypeAsync(string type)
        {
            return await _dbSet
                .Where(c => c.Type == type)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByNameAndTypeAsync(string name, string type)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Name == name && c.Type == type);
        }

        public async Task<bool> NameExistsAsync(string name, string type)
        {
            return await _dbSet
                .AnyAsync(c => c.Name == name && c.Type == type);
        }
    }
}
