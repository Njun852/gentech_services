using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gentech_services.Models;

namespace gentech_services.Repositories
{
    public interface IProductOrderRepository : IRepository<ProductOrder>
    {
        Task<IEnumerable<ProductOrder>> GetByStatusAsync(string status);
        Task<IEnumerable<ProductOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ProductOrder>> GetByCustomerAsync(string customerEmail);
        Task<ProductOrder?> GetByIdWithDetailsAsync(int productOrderId);
        Task<IEnumerable<ProductOrder>> GetRecentOrdersAsync(int count = 10);
    }
}
