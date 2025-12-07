using System.Collections.Generic;
using System.Threading.Tasks;
using gentech_services.Models;

namespace gentech_services.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetByTypeAsync(string type);
        Task<Category?> GetByNameAndTypeAsync(string name, string type);
        Task<bool> NameExistsAsync(string name, string type);
    }
}
