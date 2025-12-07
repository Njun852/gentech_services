using System.Collections.Generic;
using System.Threading.Tasks;
using gentech_services.Models;

namespace gentech_services.Repositories
{
    public interface IServiceRepository : IRepository<Service>
    {
        Task<IEnumerable<Service>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Service>> GetActiveServicesAsync();
        Task<Service?> GetByIdWithCategoryAsync(int serviceId);
        Task<IEnumerable<Service>> SearchByNameAsync(string searchTerm);
    }
}
