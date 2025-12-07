using gentech_services.Models;
using gentech_services.Data;
using gentech_services.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace gentech_services.Services
{
    public class AuthenticationService
    {
        private static AuthenticationService? instance;
        private User? currentUser;
        private UserService? _userService;
        private bool _isDatabaseInitialized = false;

        public static AuthenticationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AuthenticationService();
                }
                return instance;
            }
        }

        public User? CurrentUser
        {
            get { return currentUser; }
            private set { currentUser = value; }
        }

        public bool IsLoggedIn => currentUser != null;

        private AuthenticationService()
        {
            InitializeDatabase().Wait();
        }

        private async Task InitializeDatabase()
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<GentechDbContext>();
                optionsBuilder.UseSqlite("Data Source=gentech.db");

                using var context = new GentechDbContext(optionsBuilder.Options);

                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                var userRepository = new UserRepository(context);
                _userService = new UserService(userRepository);

                // Seed default admin user if no users exist
                var usersCount = await userRepository.CountAsync();
                if (usersCount == 0)
                {
                    await _userService.CreateUserAsync(
                        fullName: "Admin",
                        username: "Admin",
                        pin: "1234",
                        role: "Admin"
                    );
                }

                _isDatabaseInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
                _isDatabaseInitialized = false;
            }
        }

        public bool Login(string username, string pin)
        {
            return LoginAsync(username, pin).GetAwaiter().GetResult();
        }

        public async Task<bool> LoginAsync(string username, string pin)
        {
            try
            {
                if (!_isDatabaseInitialized)
                {
                    await InitializeDatabase();
                }

                var optionsBuilder = new DbContextOptionsBuilder<GentechDbContext>();
                optionsBuilder.UseSqlite("Data Source=gentech.db");

                using var context = new GentechDbContext(optionsBuilder.Options);
                var userRepository = new UserRepository(context);
                _userService = new UserService(userRepository);

                var user = await _userService.AuthenticateAsync(username, pin);

                if (user != null)
                {
                    currentUser = user;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public void Logout()
        {
            currentUser = null;
        }

        public string GetUserInitial()
        {
            if (currentUser != null && !string.IsNullOrEmpty(currentUser.Name))
            {
                return currentUser.Name[0].ToString().ToUpper();
            }
            return "U";
        }
    }
}
