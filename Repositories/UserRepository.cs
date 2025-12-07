using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gentech_services.Data;
using gentech_services.Models;

namespace gentech_services.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(GentechDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        }

        public async Task<User?> AuthenticateAsync(string username, string pin)
        {
            return await _dbSet.FirstOrDefaultAsync(u =>
                u.Username == username &&
                u.Pin == pin &&
                u.IsActive);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbSet.AnyAsync(u => u.Username == username);
        }
    }
}
