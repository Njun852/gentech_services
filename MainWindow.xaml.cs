using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using gentech_services.Views.Pages;
using gentech_services.Services;

namespace gentech_services
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Auto-login for testing purposes
            AuthenticationService.Instance.Login("Admin", "1234");

            if (AuthenticationService.Instance.IsLoggedIn)
            {
                ShowMainInterface();
                NavigateToPage("Dashboard");
            }
            else
            {
                ShowLoginPage();
            }
        }

        private void ShowLoginPage()
        {
            MainSidebar.Visibility = Visibility.Collapsed;
            SidebarColumn.Width = new GridLength(0);
            var loginPage = new LoginPage();
            loginPage.LoginSuccess += LoginPage_LoginSuccess;
            MainContentFrame.Content = loginPage;
        }

        private void ShowMainInterface()
        {
            MainSidebar.Visibility = Visibility.Visible;
            SidebarColumn.Width = new GridLength(300);
            MainSidebar.UpdateUserInfo();
        }

        private void LoginPage_LoginSuccess(object sender, System.EventArgs e)
        {
            ShowMainInterface();
            NavigateToPage("Dashboard");
        }

        private void Sidebar_NavigationRequested(object sender, string pageName)
        {
            if (pageName == "Logout")
            {
                HandleLogout();
                return;
            }

            NavigateToPage(pageName);
        }

        private void HandleLogout()
        {
            var result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Logout",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                AuthenticationService.Instance.Logout();
                ShowLoginPage();
            }
        }

        private void NavigateToPage(string pageName)
        {
            UserControl page = pageName switch
            {
                "Dashboard" => new Views.Pages.DashboardPage(),
                "ServiceManagement" => new ServiceManagementPage(),
                "ServiceOrders" => new ServiceOrderPage(),
                "InventoryManagement" => new InventoryManagementPage(),
                "InventoryLog" => new InventoryLogPage(),
                "ProductOrders" => new ProductOrdersPage(),
                "ProductOrderHistory" => new ProductOrderHistoryPage(),
                "UserManagement" => new UserManagementPage(),
                _ => new Views.Pages.DashboardPage() // Default page
            };

            MainContentFrame.Content = page;
        }
    }
}