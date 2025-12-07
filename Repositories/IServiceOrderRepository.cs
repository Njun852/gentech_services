using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gentech_services.Models;

namespace gentech_services.Repositories
{
    public interface IServiceOrderRepository : IRepository<ServiceOrder>
    {
        Task<IEnumerable<ServiceOrder>> GetByStatusAsync(string status);
        Task<IEnumerable<ServiceOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ServiceOrder>> GetByCustomerAsync(string customerEmail);
        Task<ServiceOrder?> GetByIdWithDetailsAsync(int serviceOrderId);
        Task<IEnumerable<ServiceOrder>> GetRecentOrdersAsync(int count = 10);
    }
}
