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

            viewModel.ShowEditOrderModal = (orders, services, technicians) =>
            {
                EditOrderModal.ShowModal(orders, services, technicians);
            };

            viewModel.ShowEditAppointmentModal = (order) =>
            {
                EditAppointmentModal.ShowModal(order);
            };

            // Wire up save changes callback for Edit Order Modal
            EditOrderModal.OnSaveChanges = (updatedOrder) =>
            {
                // Force UI refresh by removing and re-adding the item
                var index = viewModel.ServiceOrders.IndexOf(updatedOrder);
                if (index >= 0)
                {
                    viewModel.ServiceOrders.RemoveAt(index);
                    viewModel.ServiceOrders.Insert(index, updatedOrder);
                }

                // Refresh grouped orders to update the table status
                viewModel.RefreshGroupedOrders();
            };

            // Wire up callback for when a new service is added to an appointment
            EditOrderModal.OnServiceAdded = (newServiceOrder) =>
            {
                // Add to ViewModel's collections to persist the data
                viewModel.AddServiceOrder(newServiceOrder);
            };

            // Wire up save changes callback for Edit Appointment Modal
            EditAppointmentModal.OnSaveChanges = (updatedOrder) =>
            {
                // Force UI refresh by removing and re-adding the item
                var index = viewModel.ServiceOrders.IndexOf(updatedOrder);
                if (index >= 0)
                {
                    viewModel.ServiceOrders.RemoveAt(index);
                    viewModel.ServiceOrders.Insert(index, updatedOrder);
                }

                // Refresh grouped orders to update the table
                viewModel.RefreshGroupedOrders();
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
