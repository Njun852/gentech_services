using gentech_services.ViewsModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gentech_services.Views.Pages
{
    /// <summary>
    /// Interaction logic for ServiceOrderPage.xaml
    /// </summary>
    public partial class ServiceOrderPage : UserControl
    {
        private ServiceOrderViewModel viewModel;

        public ServiceOrderPage()
        {
            InitializeComponent();

            // Initialize database services
            var dbContext = new gentech_services.Data.GentechDbContext(
                new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<gentech_services.Data.GentechDbContext>()
                    .UseSqlite("Data Source=gentech.db")
                    .Options);

            var serviceOrderRepository = new gentech_services.Repositories.ServiceOrderRepository(dbContext);
            var serviceRepository = new gentech_services.Repositories.ServiceRepository(dbContext);
            var categoryRepository = new gentech_services.Repositories.CategoryRepository(dbContext);
            var userRepository = new gentech_services.Repositories.UserRepository(dbContext);

            var serviceOrderService = new gentech_services.Services.ServiceOrderService(serviceOrderRepository, serviceRepository);
            var serviceService = new gentech_services.Services.ServiceService(serviceRepository, categoryRepository);
            var userService = new gentech_services.Services.UserService(userRepository);

            viewModel = new ServiceOrderViewModel(serviceOrderService, serviceService, userService);
            DataContext = viewModel;

            // Wire up the modal actions
            viewModel.ShowViewOrderModal = (orders) =>
            {
                ViewOrderModal.ShowModal(orders);
            };

            // Inject ServiceOrderService into EditOrderModal
            EditOrderModal.SetServiceOrderService(serviceOrderService);

            viewModel.ShowEditOrderModal = (orders, services, technicians) =>
            {
                EditOrderModal.ShowModal(orders, services, technicians);
            };

            viewModel.ShowEditAppointmentModal = (order) =>
            {
                EditAppointmentModal.ShowModal(order);
            };

            // Wire up save changes callback for Edit Order Modal
            EditOrderModal.OnSaveChanges = async (updatedOrder) =>
            {
                // Use the ViewModel's method to handle the update and persist to database
                await viewModel.HandleEditOrderUpdate(updatedOrder);
            };

            // Wire up callback for when a new service item is added to an order
            EditOrderModal.OnServiceItemAdded = () =>
            {
                // Refresh the grouped orders to reflect the new service item
                viewModel.RefreshGroupedOrders();
            };

            // Wire up save changes callback for Edit Appointment Modal
            EditAppointmentModal.OnSaveChanges = async (updatedOrder) =>
            {
                // Use the ViewModel's method to handle the update and persist to database
                await viewModel.HandleEditAppointmentUpdate(updatedOrder);
            };

            // Subscribe to SelectableServices collection changes
            if (viewModel.SelectableServices != null)
            {
                foreach (var service in viewModel.SelectableServices)
                {
                    service.PropertyChanged += SelectableService_PropertyChanged;
                }
            }
        }

        private void SelectableService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                UpdateSelectedServicesText();
            }
        }

        private void ServicesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedServicesText();
        }

        private void UpdateSelectedServicesText()
        {
            var viewModel = DataContext as ServiceOrderViewModel;
            if (viewModel?.SelectableServices != null)
            {
                var selectedServices = viewModel.SelectableServices
                    .Where(s => s.IsSelected)
                    .Select(s => s.Name)
                    .ToList();

                if (selectedServices.Any())
                {
                    SelectedServicesText.Text = string.Join(", ", selectedServices);
                }
                else
                {
                    SelectedServicesText.Text = "Select Services";
                }
            }
        }

        private void ServicesDropdown_Click(object sender, MouseButtonEventArgs e)
        {
            if (!ServicesPopup.IsOpen)
            {
                ServicesPopup.PlacementTarget = ServicesDropdownBorder;
                ServicesPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                ServicesPopup.IsOpen = true;
            }
            else
            {
                ServicesPopup.IsOpen = false;
            }
            e.Handled = true;
        }

        private void ServicesPopup_Closed(object sender, EventArgs e)
        {
            // Popup closed
        }


    }
}
