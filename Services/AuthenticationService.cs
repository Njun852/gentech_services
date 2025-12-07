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

                // Seed default users if no users exist
                var usersCount = await userRepository.CountAsync();
                if (usersCount == 0)
                {
                    // Admin user
                    await _userService.CreateUserAsync(
                        fullName: "Admin",
                        username: "Admin",
                        pin: "1234",
                        role: "Admin"
                    );

                    // Staff user
                    await _userService.CreateUserAsync(
                        fullName: "Staff User",
                        username: "Staff",
                        pin: "1234",
                        role: "Staff"
                    );

                    // Technician user
                    await _userService.CreateUserAsync(
                        fullName: "Tech User",
                        username: "Tech",
                        pin: "1234",
                        role: "Technician"
                    );
                }

                // Seed service categories and services
                await SeedServiceCategoriesAndServices(context);

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

        public bool HasRole(string role)
        {
            return currentUser != null && currentUser.Role.Equals(role, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsAdmin()
        {
            return HasRole("Admin");
        }

        public bool IsStaff()
        {
            return HasRole("Staff");
        }

        public bool IsTechnician()
        {
            return HasRole("Technician");
        }

        public bool HasAccessToPage(string pageName)
        {
            if (currentUser == null) return false;

            // Admin has access to everything
            if (IsAdmin()) return true;

            // Dashboard is accessible by all user levels
            if (pageName.Equals("Dashboard", StringComparison.OrdinalIgnoreCase))
                return true;

            // Staff access: Service management, Service Order, Inventory Management, POS, Product Order History
            if (IsStaff())
            {
                return pageName.Equals("ServiceManagement", StringComparison.OrdinalIgnoreCase) ||
                       pageName.Equals("ServiceOrders", StringComparison.OrdinalIgnoreCase) ||
                       pageName.Equals("InventoryManagement", StringComparison.OrdinalIgnoreCase) ||
                       pageName.Equals("ProductOrders", StringComparison.OrdinalIgnoreCase) ||
                       pageName.Equals("ProductOrderHistory", StringComparison.OrdinalIgnoreCase);
            }

            // Technician access: Service Order only
            if (IsTechnician())
            {
                return pageName.Equals("ServiceOrders", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private async Task SeedServiceCategoriesAndServices(GentechDbContext context)
        {
            try
            {
                var categoryRepository = new CategoryRepository(context);
                var serviceRepository = new ServiceRepository(context);
                var categoryService = new CategoryService(categoryRepository);
                var serviceService = new ServiceService(serviceRepository, categoryRepository);

                // Check if categories already exist
                var existingCategories = await categoryService.GetServiceCategoriesAsync();
                if (existingCategories.Any())
                {
                    return; // Already seeded
                }

                // Seed Service Categories
                var repairCategory = await categoryService.CreateCategoryAsync("Repair", "Service");
                var maintenanceCategory = await categoryService.CreateCategoryAsync("Maintenance", "Service");
                var installationCategory = await categoryService.CreateCategoryAsync("Installation", "Service");
                var diagnosticCategory = await categoryService.CreateCategoryAsync("Diagnostic", "Service");

                // Seed Services for Repair Category
                await serviceService.CreateServiceAsync(
                    name: "Screen Replacement",
                    description: "Replace broken or damaged screen",
                    price: 150.00m,
                    categoryId: repairCategory.CategoryID
                );

                await serviceService.CreateServiceAsync(
                    name: "Battery Replacement",
                    description: "Replace old or defective battery",
                    price: 80.00m,
                    categoryId: repairCategory.CategoryID
                );

                await serviceService.CreateServiceAsync(
                    name: "Charging Port Repair",
                    description: "Fix or replace charging port",
                    price: 60.00m,
                    categoryId: repairCategory.CategoryID
                );

                // Seed Services for Maintenance Category
                await serviceService.CreateServiceAsync(
                    name: "General Cleaning",
                    description: "Deep clean device internals and externals",
                    price: 30.00m,
                    categoryId: maintenanceCategory.CategoryID
                );

                await serviceService.CreateServiceAsync(
                    name: "Software Update",
                    description: "Update device software to latest version",
                    price: 40.00m,
                    categoryId: maintenanceCategory.CategoryID
                );

                // Seed Services for Installation Category
                await serviceService.CreateServiceAsync(
                    name: "OS Installation",
                    description: "Install or reinstall operating system",
                    price: 100.00m,
                    categoryId: installationCategory.CategoryID
                );

                await serviceService.CreateServiceAsync(
                    name: "Software Setup",
                    description: "Install and configure essential software",
                    price: 50.00m,
                    categoryId: installationCategory.CategoryID
                );

                // Seed Services for Diagnostic Category
                await serviceService.CreateServiceAsync(
                    name: "Hardware Diagnostic",
                    description: "Complete hardware testing and diagnosis",
                    price: 35.00m,
                    categoryId: diagnosticCategory.CategoryID
                );

                await serviceService.CreateServiceAsync(
                    name: "Software Diagnostic",
                    description: "Software issue detection and analysis",
                    price: 30.00m,
                    categoryId: diagnosticCategory.CategoryID
                );

                Console.WriteLine("Service categories and services seeded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding service categories and services: {ex.Message}");
            }
        }
    }
}
