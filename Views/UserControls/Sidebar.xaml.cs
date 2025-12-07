using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using gentech_services.Services;

namespace gentech_services.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Sidebar.xaml
    /// </summary>
    public partial class Sidebar : UserControl
    {
        public event EventHandler<string> NavigationRequested;

        public Sidebar()
        {
            InitializeComponent();
            UpdateUserInfo();
        }

        public void UpdateUserInfo()
        {
            var currentUser = AuthenticationService.Instance.CurrentUser;
            if (currentUser != null)
            {
                UserNameText.Text = currentUser.Name;
                UserRoleText.Text = currentUser.Role;
                UserInitialText.Text = AuthenticationService.Instance.GetUserInitial();

                // Update menu visibility based on role
                UpdateMenuVisibility();
            }
        }

        private void UpdateMenuVisibility()
        {
            var authService = AuthenticationService.Instance;

            // Dashboard is visible to all
            DashboardButton.Visibility = Visibility.Visible;

            // Admin has access to everything
            if (authService.IsAdmin())
            {
                ServicesButton.Visibility = Visibility.Visible;
                ServiceManagementButton.Visibility = Visibility.Visible;
                ServiceOrdersButton.Visibility = Visibility.Visible;
                ProductsButton.Visibility = Visibility.Visible;
                InventoryManagementButton.Visibility = Visibility.Visible;
                POSButton.Visibility = Visibility.Visible;
                ProductOrderHistoryButton.Visibility = Visibility.Visible;
                InventoryLogButton.Visibility = Visibility.Visible;
                UsersButton.Visibility = Visibility.Visible;
                return;
            }

            // Staff access: Service management, Service Order, Inventory Management, POS, Product Order History
            if (authService.IsStaff())
            {
                ServicesButton.Visibility = Visibility.Visible;
                ServiceManagementButton.Visibility = Visibility.Visible;
                ServiceOrdersButton.Visibility = Visibility.Visible;
                ProductsButton.Visibility = Visibility.Visible;
                InventoryManagementButton.Visibility = Visibility.Visible;
                POSButton.Visibility = Visibility.Visible;
                ProductOrderHistoryButton.Visibility = Visibility.Visible;
                InventoryLogButton.Visibility = Visibility.Collapsed;
                UsersButton.Visibility = Visibility.Collapsed;
                return;
            }

            // Technician access: Service Order only
            if (authService.IsTechnician())
            {
                ServicesButton.Visibility = Visibility.Visible;
                ServiceManagementButton.Visibility = Visibility.Collapsed;
                ServiceOrdersButton.Visibility = Visibility.Visible;
                ProductsButton.Visibility = Visibility.Collapsed;
                InventoryManagementButton.Visibility = Visibility.Collapsed;
                POSButton.Visibility = Visibility.Collapsed;
                ProductOrderHistoryButton.Visibility = Visibility.Collapsed;
                InventoryLogButton.Visibility = Visibility.Collapsed;
                UsersButton.Visibility = Visibility.Collapsed;
                return;
            }

            // Default: hide all except dashboard
            ServicesButton.Visibility = Visibility.Collapsed;
            ServiceManagementButton.Visibility = Visibility.Collapsed;
            ServiceOrdersButton.Visibility = Visibility.Collapsed;
            ProductsButton.Visibility = Visibility.Collapsed;
            InventoryManagementButton.Visibility = Visibility.Collapsed;
            POSButton.Visibility = Visibility.Collapsed;
            ProductOrderHistoryButton.Visibility = Visibility.Collapsed;
            InventoryLogButton.Visibility = Visibility.Collapsed;
            UsersButton.Visibility = Visibility.Collapsed;
        }
        

        private void IconButton_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, "Dashboard");
        }

        private void Services_Click(object sender, RoutedEventArgs e)
        {
            ServicesSubButtons.Visibility = ServicesSubButtons.Visibility == Visibility.Visible
                                            ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ServiceManagement_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, "ServiceManagement");
        }

        private void ServiceOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, "ServiceOrders");
        }

        private void Products_Click(object sender, RoutedEventArgs e)
        {
            ProductsSubButtons.Visibility = ProductsSubButtons.Visibility == Visibility.Visible
                                            ? Visibility.Collapsed : Visibility.Visible;
        }

        private void InventoryManagement_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, "InventoryManagement");
        }

        private void InventoryLog_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, "InventoryLog");
        }

        private void ProductOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, "ProductOrders");
        }

        private void ProductOrderHistory_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, "ProductOrderHistory");
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, "UserManagement");
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, "Logout");
        }

        private void IconButton_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

}
