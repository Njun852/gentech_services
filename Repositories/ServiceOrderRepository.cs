using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gentech_services.Data;
using gentech_services.Models;

namespace gentech_services.Repositories
{
    public class ServiceOrderRepository : Repository<ServiceOrder>, IServiceOrderRepository
    {
        public ServiceOrderRepository(GentechDbContext context) : base(context) { }

        public async Task<IEnumerable<ServiceOrder>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Include(so => so.ServiceOrderItems)
                    .ThenInclude(soi => soi.Service)
                .Where(so => so.Status == status)
                .OrderByDescending(so => so.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(so => so.ServiceOrderItems)
                    .ThenInclude(soi => soi.Service)
                .Where(so => so.ScheduledAt >= startDate && so.ScheduledAt <= endDate)
                .OrderByDescending(so => so.ScheduledAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceOrder>> GetByCustomerAsync(string customerEmail)
        {
            return await _dbSet
                .Include(so => so.ServiceOrderItems)
                    .ThenInclude(soi => soi.Service)
                .Where(so => so.Email == customerEmail)
                .OrderByDescending(so => so.CreatedAt)
                .ToListAsync();
        }

        public async Task<ServiceOrder?> GetByIdWithDetailsAsync(int serviceOrderId)
        {
            return await _dbSet
                .Include(so => so.ServiceOrderItems)
                    .ThenInclude(soi => soi.Service)
                        .ThenInclude(s => s.Category)
                .FirstOrDefaultAsync(so => so.ServiceOrderID == serviceOrderId);
        }

        public async Task<IEnumerable<ServiceOrder>> GetRecentOrdersAsync(int count = 10)
        {
            return await _dbSet
                .Include(so => so.ServiceOrderItems)
                    .ThenInclude(soi => soi.Service)
                .OrderByDescending(so => so.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public override async Task<ServiceOrder?> GetByIdAsync(int id)
        {
            return await GetByIdWithDetailsAsync(id);
        }

        public override async Task<IEnumerable<ServiceOrder>> GetAllAsync()
        {
            return await _dbSet
                .Include(so => so.ServiceOrderItems)
                    .ThenInclude(soi => soi.Service)
                        .ThenInclude(s => s.Category)
                .OrderByDescending(so => so.CreatedAt)
                .ToListAsync();
        }
    }
}
