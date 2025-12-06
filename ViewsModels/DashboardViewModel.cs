using gentech_services.Helpers;
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
            // Load today's sales
            var today = DateTime.Today;
            var todaySales = SaleManager.GetAllSales()
                .Where(s => s.SaleDate.Date == today && s.SaleType == "Product")
                .ToList();
            TodaysSales = todaySales.Sum(s => s.TotalAmount);

            // Load low stock items (stock <= 10)
            var lowStock = ProductManager.GetAllProducts()
                .Where(p => p.Stock <= 10)
                .OrderBy(p => p.Stock)
                .ToList();
            LowStockItems.Clear();
            foreach (var product in lowStock)
            {
                LowStockItems.Add(product);
            }
            LowStockCount = lowStock.Count;

            // Load open service jobs
            var serviceJobs = SaleManager.GetAllSales()
                .Where(s => s.SaleType == "Service" && (s.Status == "Pending" || s.Status == "Ongoing"))
                .OrderByDescending(s => s.SaleDate)
                .ToList();
            OpenServiceJobs = serviceJobs.Count;

            PendingServiceJobs.Clear();
            foreach (var job in serviceJobs)
            {
                PendingServiceJobs.Add(job);
            }

            // Load total transactions count
            TotalTransactions = SaleManager.GetAllSales().Count;

            // Load recent transactions (last 10)
            var recent = SaleManager.GetAllSales()
                .OrderByDescending(s => s.SaleDate)
                .Take(10)
                .ToList();
            RecentTransactions.Clear();
            foreach (var transaction in recent)
            {
                RecentTransactions.Add(transaction);
            }
        }

        private void NavigateToPOS(object parameter)
        {
            // Navigation logic will be implemented by MainWindow
            MainWindow.Instance?.NavigateToPage("POS");
        }

        private void NavigateToInventory(object parameter)
        {
            MainWindow.Instance?.NavigateToPage("Inventory");
        }

        private void NavigateToProductOrders(object parameter)
        {
            MainWindow.Instance?.NavigateToPage("ProductOrders");
        }

        private void NavigateToServiceOrders(object parameter)
        {
            MainWindow.Instance?.NavigateToPage("ServiceOrders");
        }

        private void NavigateToUsers(object parameter)
        {
            MainWindow.Instance?.NavigateToPage("Users");
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
