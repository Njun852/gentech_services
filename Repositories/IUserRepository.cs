using System.Threading.Tasks;
using gentech_services.Models;

namespace gentech_services.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> AuthenticateAsync(string username, string pin);
        Task<bool> UsernameExistsAsync(string username);
    }
}
