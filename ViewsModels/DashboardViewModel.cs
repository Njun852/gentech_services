using gentech_services.Models;
using gentech_services.MVVM;
using ProductServicesManagementSystem.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace gentech_services.ViewsModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private decimal todaysSales;
        private int lowStockCount;
        private int openServiceJobs;
        private int totalTransactions;

        public decimal TodaysSales
        {
            get { return todaysSales; }
            set
            {
                todaysSales = value;
                OnPropertyChanged(nameof(TodaysSales));
            }
        }

        public int LowStockCount
        {
            get { return lowStockCount; }
            set
            {
                lowStockCount = value;
                OnPropertyChanged(nameof(LowStockCount));
            }
        }

        public int OpenServiceJobs
        {
            get { return openServiceJobs; }
            set
            {
                openServiceJobs = value;
                OnPropertyChanged(nameof(OpenServiceJobs));
            }
        }

        public int TotalTransactions
        {
            get { return totalTransactions; }
            set
            {
                totalTransactions = value;
                OnPropertyChanged(nameof(TotalTransactions));
            }
        }

        public ObservableCollection<Product> LowStockItems { get; set; }
        public ObservableCollection<Sale> RecentTransactions { get; set; }
        public ObservableCollection<Sale> PendingServiceJobs { get; set; }

        public ICommand NavigateToPOSCommand { get; }
        public ICommand NavigateToInventoryCommand { get; }
        public ICommand NavigateToProductOrdersCommand { get; }
        public ICommand NavigateToServiceOrdersCommand { get; }
        public ICommand NavigateToUsersCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public DashboardViewModel()
        {
            LowStockItems = new ObservableCollection<Product>();
            RecentTransactions = new ObservableCollection<Sale>();
            PendingServiceJobs = new ObservableCollection<Sale>();

            NavigateToPOSCommand = new RelayCommand(NavigateToPOS);
            NavigateToInventoryCommand = new RelayCommand(NavigateToInventory);
            NavigateToProductOrdersCommand = new RelayCommand(NavigateToProductOrders);
            NavigateToServiceOrdersCommand = new RelayCommand(NavigateToServiceOrders);
            NavigateToUsersCommand = new RelayCommand(NavigateToUsers);

            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            // Mock data for dashboard metrics
            TodaysSales = 45750.00m;
            LowStockCount = 8;
            OpenServiceJobs = 12;
            TotalTransactions = 24;

            // Mock low stock items
            LowStockItems.Add(new Product
            {
                ProductID = 1,
                Name = "iPhone 14 Pro",
                SKU = "IPH14P-256",
                Description = "256GB, Space Black",
                Price = 54990.00m,
                StockQuanity = 3,
                IsActive = true,
                CreatedAt = DateTime.Now,
                Category = new Category { CategoryID = 1, Name = "Smartphones", Type = "Product" }
            });

            LowStockItems.Add(new Product
            {
                ProductID = 2,
                Name = "Samsung Galaxy S23",
                SKU = "SGS23-128",
                Description = "128GB, Phantom Black",
                Price = 49990.00m,
                StockQuanity = 2,
                IsActive = true,
                CreatedAt = DateTime.Now,
                Category = new Category { CategoryID = 1, Name = "Smartphones", Type = "Product" }
            });

            LowStockItems.Add(new Product
            {
                ProductID = 3,
                Name = "MacBook Air M2",
                SKU = "MBA-M2-256",
                Description = "13-inch, 256GB SSD",
                Price = 64990.00m,
                StockQuanity = 1,
                IsActive = true,
                CreatedAt = DateTime.Now,
                Category = new Category { CategoryID = 2, Name = "Laptops", Type = "Product" }
            });

            LowStockItems.Add(new Product
            {
                ProductID = 4,
                Name = "AirPods Pro 2",
                SKU = "APP2-GEN2",
                Description = "2nd Generation",
                Price = 12990.00m,
                StockQuanity = 4,
                IsActive = true,
                CreatedAt = DateTime.Now,
                Category = new Category { CategoryID = 3, Name = "Accessories", Type = "Product" }
            });

            LowStockItems.Add(new Product
            {
                ProductID = 5,
                Name = "iPad Pro 11\"",
                SKU = "IPP11-256",
                Description = "11-inch, 256GB, Wi-Fi",
                Price = 44990.00m,
                StockQuanity = 2,
                IsActive = true,
                CreatedAt = DateTime.Now,
                Category = new Category { CategoryID = 4, Name = "Tablets", Type = "Product" }
            });

            // Mock recent transactions
            RecentTransactions.Add(new Sale
            {
                SaleID = 101,
                CreatedAt = DateTime.Now.AddMinutes(-30),
                TotalAmount = 54990.00m,
                PaymentMethod = "Credit Card",
                Status = "Completed",
                CustomerID = 1,
                StaffID = 1,
                Customer = new Customer { FirstName = "John", LastName = "Doe", Email = "john.doe@email.com", Phone = "0912-345-6789" },
                Staff = new User { UserID = 1, Name = "Nicole Juntilla", Role = "Technician" }
            });

            RecentTransactions.Add(new Sale
            {
                SaleID = 102,
                CreatedAt = DateTime.Now.AddHours(-1),
                TotalAmount = 12990.00m,
                PaymentMethod = "Cash",
                Status = "Completed",
                CustomerID = 2,
                StaffID = 1,
                Customer = new Customer { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@email.com", Phone = "0923-456-7890" },
                Staff = new User { UserID = 1, Name = "Nicole Juntilla", Role = "Technician" }
            });

            RecentTransactions.Add(new Sale
            {
                SaleID = 103,
                CreatedAt = DateTime.Now.AddHours(-2),
                TotalAmount = 49990.00m,
                PaymentMethod = "GCash",
                Status = "Completed",
                CustomerID = 3,
                StaffID = 1,
                Customer = new Customer { FirstName = "Mike", LastName = "Johnson", Email = "mike.j@email.com", Phone = "0934-567-8901" },
                Staff = new User { UserID = 1, Name = "Nicole Juntilla", Role = "Technician" }
            });

            RecentTransactions.Add(new Sale
            {
                SaleID = 104,
                CreatedAt = DateTime.Now.AddHours(-3),
                TotalAmount = 64990.00m,
                PaymentMethod = "Credit Card",
                Status = "Completed",
                CustomerID = 4,
                StaffID = 1,
                Customer = new Customer { FirstName = "Sarah", LastName = "Williams", Email = "sarah.w@email.com", Phone = "0945-678-9012" },
                Staff = new User { UserID = 1, Name = "Nicole Juntilla", Role = "Technician" }
            });

            RecentTransactions.Add(new Sale
            {
                SaleID = 105,
                CreatedAt = DateTime.Now.AddHours(-4),
                TotalAmount = 25980.00m,
                PaymentMethod = "Cash",
                Status = "Completed",
                CustomerID = 5,
                StaffID = 1,
                Customer = new Customer { FirstName = "David", LastName = "Brown", Email = "david.b@email.com", Phone = "0956-789-0123" },
                Staff = new User { UserID = 1, Name = "Nicole Juntilla", Role = "Technician" }
            });

            // Mock pending service jobs
            PendingServiceJobs.Add(new Sale
            {
                SaleID = 201,
                CreatedAt = DateTime.Now.AddDays(-1),
                TotalAmount = 2500.00m,
                PaymentMethod = "Cash",
                Status = "Pending",
                CustomerID = 6,
                StaffID = 1,
                Customer = new Customer { FirstName = "Emily", LastName = "Davis", Email = "emily.d@email.com", Phone = "0967-890-1234" },
                Staff = new User { UserID = 1, Name = "Nicole Juntilla", Role = "Technician" }
            });

            PendingServiceJobs.Add(new Sale
            {
                SaleID = 202,
                CreatedAt = DateTime.Now.AddDays(-1),
                TotalAmount = 3500.00m,
                PaymentMethod = "GCash",
                Status = "Pending",
                CustomerID = 7,
                StaffID = 1,
                Customer = new Customer { FirstName = "Chris", LastName = "Martinez", Email = "chris.m@email.com", Phone = "0978-901-2345" },
                Staff = new User { UserID = 1, Name = "Nicole Juntilla", Role = "Technician" }
            });

            PendingServiceJobs.Add(new Sale
            {
                SaleID = 203,
                CreatedAt = DateTime.Now.AddDays(-2),
                TotalAmount = 1800.00m,
                PaymentMethod = "Cash",
                Status = "Ongoing",
                CustomerID = 8,
                StaffID = 1,
                Customer = new Customer { FirstName = "Lisa", LastName = "Garcia", Email = "lisa.g@email.com", Phone = "0989-012-3456" },
                Staff = new User { UserID = 1, Name = "Nicole Juntilla", Role = "Technician" }
            });

            PendingServiceJobs.Add(new Sale
            {
                SaleID = 204,
                CreatedAt = DateTime.Now.AddDays(-2),
                TotalAmount = 4200.00m,
                PaymentMethod = "Credit Card",
                Status = "Ongoing",
                CustomerID = 9,
                StaffID = 1,
                Customer = new Customer { FirstName = "Robert", LastName = "Lopez", Email = "robert.l@email.com", Phone = "0990-123-4567" },
                Staff = new User { UserID = 1, Name = "Nicole Juntilla", Role = "Technician" }
            });

            PendingServiceJobs.Add(new Sale
            {
                SaleID = 205,
                CreatedAt = DateTime.Now.AddDays(-3),
                TotalAmount = 2800.00m,
                PaymentMethod = "Cash",
                Status = "Pending",
                CustomerID = 10,
                StaffID = 1,
                Customer = new Customer { FirstName = "Amanda", LastName = "Taylor", Email = "amanda.t@email.com", Phone = "0901-234-5678" },
                Staff = new User { UserID = 1, Name = "Nicole Juntilla", Role = "Technician" }
            });
        }

        private void NavigateToPOS(object parameter)
        {
        
        }

        private void NavigateToInventory(object parameter)
        {
        }

        private void NavigateToProductOrders(object parameter)
        {
        }

        private void NavigateToServiceOrders(object parameter)
        {
        }

        private void NavigateToUsers(object parameter)
        {
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
