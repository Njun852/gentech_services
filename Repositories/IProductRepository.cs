using System.Collections.Generic;
using System.Threading.Tasks;
using gentech_services.Models;

namespace gentech_services.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetBySKUAsync(string sku);
        Task<IEnumerable<Product>> GetLowStockProductsAsync();
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<bool> SKUExistsAsync(string sku);
    }
}
