using gentech_services.Models;
using gentech_services.MVVM;
using gentech_services.Services;
using ProductServicesManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace gentech_services.ViewsModels
{
    // Grouped Service Order class for displaying service orders
    public class GroupedServiceOrder : INotifyPropertyChanged
    {
        public int ServiceOrderID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Status { get; set; }
        public ServiceOrder Order { get; set; }

        public string ServicesDisplay
        {
            get
            {
                if (Order?.ServiceOrderItems == null || !Order.ServiceOrderItems.Any()) return "";

                var items = Order.ServiceOrderItems.ToList();
                if (items.Count <= 2)
                {
                    return string.Join(", ", items.Select(soi => soi.Service?.Name));
                }
                else
                {
                    var firstTwo = string.Join(", ", items.Take(2).Select(soi => soi.Service?.Name));
                    return $"{firstTwo} +{items.Count - 2} more";
                }
            }
        }

        public decimal TotalPrice
        {
            get
            {
                return Order?.ServiceOrderItems?.Sum(soi => soi.TotalPrice) ?? 0;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal class ServiceOrderViewModel : ViewModelBase
    {
        private readonly ServiceOrderService _serviceOrderService;
        private readonly ServiceService _serviceService;
        private readonly UserService _userService;

        private Appointment currentAppointment;
        private ObservableCollection<ServiceOrder> serviceOrders;
        private ObservableCollection<ServiceOrder> allServiceOrders; // Store unfiltered list
        private ObservableCollection<GroupedServiceOrder> groupedServiceOrders;
        private ServiceOrder selectedOrder;
        private ObservableCollection<User> availableTechnicians;

        // Action to trigger modal display from View
        public Action<List<ServiceOrder>> ShowViewOrderModal { get; set; }
        public Action<List<ServiceOrder>, ObservableCollection<Service>, ObservableCollection<User>> ShowEditOrderModal { get; set; }
        public Action<ServiceOrder> ShowEditAppointmentModal { get; set; }

        private string customerName;
        private string email;
        private string phone;
        private Service selectedService;
        private DateTime? selectedDate;
        private string issueDescription;
        private ObservableCollection<SelectableService> selectableServices;
        private ObservableCollection<OrderServiceItem> orderServices;
        private ObservableCollection<Service> availableServicesToAdd;
        private bool isServiceSelectorVisible;
        private User selectedTechnician;

        private string customerNameError;
        private string emailError;
        private string phoneError;
        private string serviceError;
        private string dateError;

        private string selectedStatusFilter;
        private User selectedTechnicianFilter;
        private ObservableCollection<string> statusFilters;

        public ObservableCollection<ServiceOrder> ServiceOrders {
            get { return serviceOrders; } }

        public ObservableCollection<GroupedServiceOrder> GroupedServiceOrders
        {
            get { return groupedServiceOrders; }
            set { groupedServiceOrders = value; OnPropertyChanged(); }
        }

        public ServiceOrder SelectedOrder
        {
            get { return selectedOrder; }
            set { selectedOrder = value; OnPropertyChanged(); }
        }

        public Appointment CurrentAppointment
        {
            get { return currentAppointment; }
            set { currentAppointment = value; OnPropertyChanged(); }
        }

        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get { return email; }
            set { email = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; OnPropertyChanged(); }
        }

        public Service SelectedService
        {
            get { return selectedService; }
            set { selectedService = value; OnPropertyChanged(); }
        }

        public DateTime? SelectedDate
        {
            get { return selectedDate; }
            set { selectedDate = value; OnPropertyChanged(); }
        }

        public string IssueDescription
        {
            get { return issueDescription; }
            set { issueDescription = value; OnPropertyChanged(); }
        }

        public ObservableCollection<SelectableService> SelectableServices
        {
            get { return selectableServices; }
            set { selectableServices = value; OnPropertyChanged(); }
        }

        public ObservableCollection<OrderServiceItem> OrderServices
        {
            get { return orderServices; }
            set { orderServices = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Service> AvailableServicesToAdd
        {
            get { return availableServicesToAdd; }
            set { availableServicesToAdd = value; OnPropertyChanged(); }
        }

        public bool IsServiceSelectorVisible
        {
            get { return isServiceSelectorVisible; }
            set { isServiceSelectorVisible = value; OnPropertyChanged(); }
        }

        public ObservableCollection<User> AvailableTechnicians
        {
            get { return availableTechnicians; }
            set { availableTechnicians = value; OnPropertyChanged(); }
        }

        public User SelectedTechnician
        {
            get { return selectedTechnician; }
            set { selectedTechnician = value; OnPropertyChanged(); }
        }

        public string CustomerNameError
        {
            get { return customerNameError; }
            set { customerNameError = value; OnPropertyChanged(); }
        }

        public string EmailError
        {
            get { return emailError; }
            set { emailError = value; OnPropertyChanged(); }
        }

        public string PhoneError
        {
            get { return phoneError; }
            set { phoneError = value; OnPropertyChanged(); }
        }

        public string ServiceError
        {
            get { return serviceError; }
            set { serviceError = value; OnPropertyChanged(); }
        }

        public string DateError
        {
            get { return dateError; }
            set { dateError = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> StatusFilters
        {
            get { return statusFilters; }
            set { statusFilters = value; OnPropertyChanged(); }
        }

        public string SelectedStatusFilter
        {
            get { return selectedStatusFilter; }
            set
            {
                selectedStatusFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public User SelectedTechnicianFilter
        {
            get { return selectedTechnicianFilter; }
            set
            {
                selectedTechnicianFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }


        public RelayCommand ViewDetailsCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand EditAppointmentCommand { get; private set; }
        public RelayCommand SetToOngoingCommand { get; private set; }
        public RelayCommand SetToCompletedCommand { get; private set; }
        public RelayCommand CancelAppointmentCommand { get; private set; }
        public RelayCommand RescheduleCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand SetAppointmentCommand { get; private set; }
        public RelayCommand ClearFormCommand { get; private set; }
        public RelayCommand ShowServiceSelectorCommand { get; private set; }
        public RelayCommand AddServiceToOrderCommand { get; private set; }
        public RelayCommand RemoveServiceFromOrderCommand { get; private set; }

        public ServiceOrderViewModel(ServiceOrderService serviceOrderService, ServiceService serviceService, UserService userService)
        {
            _serviceOrderService = serviceOrderService;
            _serviceService = serviceService;
            _userService = userService;

            currentAppointment = new Appointment();
            serviceOrders = new ObservableCollection<ServiceOrder>();
            allServiceOrders = new ObservableCollection<ServiceOrder>();
            groupedServiceOrders = new ObservableCollection<GroupedServiceOrder>();
            orderServices = new ObservableCollection<OrderServiceItem>();
            availableServicesToAdd = new ObservableCollection<Service>();
            selectableServices = new ObservableCollection<SelectableService>();

            statusFilters = new ObservableCollection<string>
            {
                "All Statuses",
                "Pending",
                "In Progress",
                "Completed",
                "Cancelled"
            };
            selectedStatusFilter = "All Statuses";

            ViewDetailsCommand = new RelayCommand(obj => ViewDetails(obj as ServiceOrder));
            EditCommand = new RelayCommand(obj => Edit(obj as ServiceOrder));
            EditAppointmentCommand = new RelayCommand(obj => EditAppointment(obj as ServiceOrder));
            SetToOngoingCommand = new RelayCommand(obj => SetToOngoing(obj as ServiceOrder), obj => obj is ServiceOrder);
            SetToCompletedCommand = new RelayCommand(obj => SetToCompleted(obj as ServiceOrder), obj => obj is ServiceOrder);
            CancelAppointmentCommand = new RelayCommand(obj => CancelAppointment(obj as ServiceOrder), obj => obj is ServiceOrder);
            RescheduleCommand = new RelayCommand(obj => Reschedule(obj as ServiceOrder));
            DeleteCommand = new RelayCommand(obj => Delete(obj as ServiceOrder), obj => obj is ServiceOrder);
            SetAppointmentCommand = new RelayCommand(obj => SetAppointment(), obj => CanSetAppointment());
            ClearFormCommand = new RelayCommand(obj => ClearForm());
            ShowServiceSelectorCommand = new RelayCommand(obj => ShowServiceSelector());
            AddServiceToOrderCommand = new RelayCommand(obj => AddServiceToOrder(obj as Service));
            RemoveServiceFromOrderCommand = new RelayCommand(obj => RemoveServiceFromOrder(obj as OrderServiceItem));

            _ = LoadDataFromDatabase();
        }
        private void ViewDetails(ServiceOrder order)
        {
            if (order != null)
            {
                SelectedOrder = order;
                // In the new schema, each service order contains multiple service items
                ShowViewOrderModal?.Invoke(new List<ServiceOrder> { order });
            }
        }

        private void Edit(ServiceOrder order)
        {
            if (order != null)
            {
                SelectedOrder = order;
                // In the new schema, each service order contains multiple service items
                var services = new ObservableCollection<Service>(selectableServices.Select(s => s.Service));
                ShowEditOrderModal?.Invoke(new List<ServiceOrder> { order }, services, availableTechnicians);
            }
        }

        private void EditAppointment(ServiceOrder order)
        {
            if (order != null)
            {
                SelectedOrder = order;
                ShowEditAppointmentModal?.Invoke(order);
            }
        }

        private async void CancelAppointment(ServiceOrder order)
        {
            if (order != null)
            {
                if (order.Status == "Completed")
                {
                    MessageBox.Show(
                        "Cannot cancel a completed order.",
                        "Cancel Not Allowed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"Are you sure you want to cancel this appointment?\n\n" +
                    $"Order ID: S{order.ServiceOrderID:000}\n" +
                    $"Customer: {order.FullName}\n" +
                    $"Scheduled: {order.ScheduledAt:dd/MM/yyyy}",
                    "Cancel Appointment",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _serviceOrderService.UpdateStatusAsync(order.ServiceOrderID, "Cancelled");
                        order.Status = "Cancelled";

                        // Refresh UI
                        RefreshGroupedOrders();

                        MessageBox.Show(
                            "Appointment cancelled successfully.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Failed to cancel appointment: {ex.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void SetToOngoing(ServiceOrder order)
        {
            if (order != null)
            {
                if (order.Status == "In Progress")
                {
                    MessageBox.Show(
                        "This order is already in progress.",
                        "Already In Progress",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                if (order.Status == "Completed")
                {
                    MessageBox.Show(
                        "Cannot change status of a completed order.",
                        "Action Not Allowed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                if (order.Status == "Cancelled")
                {
                    MessageBox.Show(
                        "Cannot change status of a cancelled order.",
                        "Action Not Allowed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"Start working on this order?\n\n" +
                    $"Order ID: S{order.ServiceOrderID:000}\n" +
                    $"Customer: {order.FullName}\n" +
                    $"Scheduled: {order.ScheduledAt:dd/MM/yyyy}",
                    "Set to In Progress",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _serviceOrderService.UpdateStatusAsync(order.ServiceOrderID, "In Progress");
                        order.Status = "In Progress";

                        // Refresh UI
                        RefreshGroupedOrders();

                        MessageBox.Show(
                            "Order status updated to In Progress.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Failed to update order status: {ex.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void SetToCompleted(ServiceOrder order)
        {
            if (order != null)
            {
                if (order.Status == "Completed")
                {
                    MessageBox.Show(
                        "This order is already completed.",
                        "Already Completed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                if (order.Status == "Pending")
                {
                    MessageBox.Show(
                        "Cannot complete a pending order. Please set it to In Progress first.",
                        "Action Not Allowed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                if (order.Status == "Cancelled")
                {
                    MessageBox.Show(
                        "Cannot mark a cancelled order as completed.",
                        "Action Not Allowed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"Mark this order as completed?\n\n" +
                    $"Order ID: S{order.ServiceOrderID:000}\n" +
                    $"Customer: {order.FullName}\n" +
                    $"Scheduled: {order.ScheduledAt:dd/MM/yyyy}",
                    "Set to Completed",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _serviceOrderService.UpdateStatusAsync(order.ServiceOrderID, "Completed");
                        order.Status = "Completed";

                        // Refresh UI
                        RefreshGroupedOrders();

                        MessageBox.Show(
                            "Order marked as completed successfully.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Failed to update order status: {ex.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Reschedule(ServiceOrder order)
        {
            if (order != null)
            {
                if (order.Status == "Completed")
                {
                    MessageBox.Show(
                        "Cannot reschedule a completed order.",
                        "Action Not Allowed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                if (order.Status == "Cancelled")
                {
                    MessageBox.Show(
                        "Cannot reschedule a cancelled order.",
                        "Action Not Allowed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }


                MessageBox.Show(
                    $"Reschedule feature coming soon.\n\n" +
                    $"Order ID: #S{order.SaleID:000}\n" +
                    $"Current Date: {order.AppointmentDate:dd/MM/yyyy}\n\n" +
                    $"Please use the Edit option to change the appointment date.",
                    "Reschedule Order",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private async void Delete(ServiceOrder order)
        {
            if (order != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete this order? This action cannot be undone.\n\n" +
                    $"Order ID: S{order.ServiceOrderID:000}\n" +
                    $"Customer: {order.FullName}\n" +
                    $"Scheduled: {order.ScheduledAt:dd/MM/yyyy}",
                    "Delete Order",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _serviceOrderService.DeleteServiceOrderAsync(order.ServiceOrderID);

                        allServiceOrders.Remove(order);
                        serviceOrders.Remove(order);

                        // Refresh UI
                        RefreshGroupedOrders();

                        MessageBox.Show(
                            "Order deleted successfully.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Failed to delete order: {ex.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ApplyFilters()
        {
            IEnumerable<ServiceOrder> filteredOrders = allServiceOrders;

            if (!string.IsNullOrEmpty(SelectedStatusFilter) && SelectedStatusFilter != "All Statuses")
            {
                filteredOrders = filteredOrders.Where(o => o.Status == SelectedStatusFilter);
            }

            if (SelectedTechnicianFilter != null && SelectedTechnicianFilter.FullName != "All Technicians")
            {
                // Note: Technician assignment not implemented yet in new schema
                // This filter will be implemented when technician assignment is added
            }

            serviceOrders.Clear();
            foreach (var order in filteredOrders)
            {
                serviceOrders.Add(order);
            }

            // Update grouped orders
            UpdateGroupedOrders();
        }

        private void UpdateGroupedOrders()
        {
            groupedServiceOrders.Clear();

            // Each service order is now a single entity in the new schema
            var grouped = serviceOrders.Select(o => new GroupedServiceOrder
            {
                ServiceOrderID = o.ServiceOrderID,
                FullName = o.FullName,
                Email = o.Email,
                Phone = o.Phone,
                ScheduledAt = o.ScheduledAt,
                Status = o.Status,
                Order = o
            }).OrderByDescending(g => g.ScheduledAt);

            foreach (var group in grouped)
            {
                groupedServiceOrders.Add(group);
            }
        }

        // Public method to refresh grouped orders (called after editing services)
        public void RefreshGroupedOrders()
        {
            UpdateGroupedOrders();
        }

        // Public method to add a new service order to the collections
        public void AddServiceOrder(ServiceOrder newServiceOrder)
        {
            if (newServiceOrder == null) return;

            // Add to both collections to ensure it persists
            allServiceOrders.Add(newServiceOrder);
            serviceOrders.Add(newServiceOrder);

            // Refresh grouped orders to update the table
            UpdateGroupedOrders();
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            CustomerNameError = string.Empty;
            EmailError = string.Empty;
            PhoneError = string.Empty;
            ServiceError = string.Empty;
            DateError = string.Empty;

            if (string.IsNullOrWhiteSpace(CustomerName))
            {
                CustomerNameError = "Customer name is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = "Email is required";
                isValid = false;
            }
            else if (!IsValidEmail(Email))
            {
                EmailError = "Invalid email format";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                PhoneError = "Phone number is required";
                isValid = false;
            }
            else if (!IsValidPhone(Phone))
            {
                PhoneError = "Invalid phone number format";
                isValid = false;
            }

            if (OrderServices.Count == 0)
            {
                ServiceError = "Please add at least one service";
                isValid = false;
            }

            if (!SelectedDate.HasValue)
            {
                DateError = "Date is required";
                isValid = false;
            }
            else if (SelectedDate.Value.Date < DateTime.Now.Date)
            {
                DateError = "Date cannot be in the past";
                isValid = false;
            }

            return isValid;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            string cleaned = phone.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "");

            return cleaned.Length >= 10 && cleaned.Length <= 11 && cleaned.All(char.IsDigit);
        }

        private bool CanSetAppointment()
        {
            return !string.IsNullOrWhiteSpace(CustomerName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Phone) &&
                   OrderServices.Count > 0 &&
                   SelectedDate.HasValue;
        }

        private async void SetAppointment()
        {
            if (!ValidateForm())
            {
                return;
            }

            try
            {
                // Prepare service items with quantity and unit price
                var serviceItems = OrderServices.Select(os => (
                    ServiceID: os.Service.ServiceID,
                    Quantity: 1,
                    UnitPrice: os.Service.Price
                )).ToList();

                // Create service order in database
                var createdOrder = await _serviceOrderService.CreateServiceOrderAsync(
                    fullName: CustomerName.Trim(),
                    email: Email.Trim(),
                    phone: Phone.Trim(),
                    scheduledAt: SelectedDate.Value,
                    deviceDescription: null, // Can be added to UI later if needed
                    issueNotes: IssueDescription,
                    serviceItems: serviceItems
                );

                // Add to local collections
                allServiceOrders.Add(createdOrder);
                serviceOrders.Add(createdOrder);

                // Update grouped orders to refresh the table
                UpdateGroupedOrders();

                // Calculate total cost
                decimal totalCost = createdOrder.ServiceOrderItems.Sum(soi => soi.TotalPrice);

                // Show success message
                MessageBox.Show($"Appointment created successfully!\n\n" +
                               $"Order ID: S{createdOrder.ServiceOrderID:000}\n" +
                               $"Customer: {CustomerName}\n" +
                               $"Services: {createdOrder.ServiceOrderItems.Count}\n" +
                               $"Total Cost: ₱{totalCost:N2}\n" +
                               $"Date: {SelectedDate.Value:dd/MM/yyyy}",
                               "Success",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);

                // Clear form
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to create appointment: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ShowServiceSelector()
        {
            UpdateAvailableServicesToAdd();
            IsServiceSelectorVisible = true;
        }

        private void AddServiceToOrder(Service service)
        {
            if (service != null && !OrderServices.Any(os => os.Service.ServiceID == service.ServiceID))
            {
                var orderServiceItem = new OrderServiceItem
                {
                    Service = service,
                    Technician = new User { Name = "Select Technician" },
                    Status = "Pending"
                };
                OrderServices.Add(orderServiceItem);
                UpdateAvailableServicesToAdd();
                IsServiceSelectorVisible = false;
            }
        }

        private void RemoveServiceFromOrder(OrderServiceItem orderServiceItem)
        {
            if (orderServiceItem != null)
            {
                OrderServices.Remove(orderServiceItem);
                UpdateAvailableServicesToAdd();
            }
        }

        private void UpdateAvailableServicesToAdd()
        {
            AvailableServicesToAdd.Clear();
            foreach (var selectableService in selectableServices)
            {
                if (!OrderServices.Any(os => os.Service.ServiceID == selectableService.Service.ServiceID))
                {
                    AvailableServicesToAdd.Add(selectableService.Service);
                }
            }
        }

        private void SelectableService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectableService.IsSelected))
            {
                var selectableService = sender as SelectableService;
                if (selectableService != null)
                {
                    if (selectableService.IsSelected)
                    {
                        // Add to OrderServices if not already there
                        if (!OrderServices.Any(os => os.Service.ServiceID == selectableService.Service.ServiceID))
                        {
                            var orderServiceItem = new OrderServiceItem
                            {
                                Service = selectableService.Service,
                                Status = "Pending"
                            };
                            OrderServices.Add(orderServiceItem);
                        }
                    }
                    else
                    {
                        // Remove from OrderServices
                        var itemToRemove = OrderServices.FirstOrDefault(os => os.Service.ServiceID == selectableService.Service.ServiceID);
                        if (itemToRemove != null)
                        {
                            OrderServices.Remove(itemToRemove);
                        }
                    }
                }
            }
        }

        private void ClearForm()
        {
            CustomerName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            SelectedService = null;
            SelectedDate = null;
            IssueDescription = string.Empty;
            SelectedTechnician = null;
            OrderServices.Clear();

            // Uncheck all selectable services
            foreach (var service in SelectableServices)
            {
                service.IsSelected = false;
            }

            UpdateAvailableServicesToAdd();

            // Clear errors
            CustomerNameError = string.Empty;
            EmailError = string.Empty;
            PhoneError = string.Empty;
            ServiceError = string.Empty;
            DateError = string.Empty;
        }

        private async Task LoadDataFromDatabase()
        {
            try
            {
                // Load services from database
                var services = await _serviceService.GetActiveServicesAsync();
                selectableServices = new ObservableCollection<SelectableService>();
                foreach (var service in services)
                {
                    var selectableService = new SelectableService
                    {
                        Service = service,
                        IsSelected = false
                    };
                    selectableService.PropertyChanged += SelectableService_PropertyChanged;
                    selectableServices.Add(selectableService);
                }
                OnPropertyChanged(nameof(SelectableServices));

                UpdateAvailableServicesToAdd();

                // Load all users and filter technicians
                var allUsers = await _userService.GetAllUsersAsync();
                var technicians = allUsers.Where(u => u.Role == "Technician" && u.IsActive).ToList();

                availableTechnicians = new ObservableCollection<User>
                {
                    new User { FullName = "All Technicians", Username = "filter" } // Filter option
                };
                foreach (var tech in technicians)
                {
                    availableTechnicians.Add(tech);
                }
                selectedTechnicianFilter = availableTechnicians[0];
                OnPropertyChanged(nameof(AvailableTechnicians));

                // Load service orders from database
                var orders = await _serviceOrderService.GetAllServiceOrdersAsync();
                allServiceOrders.Clear();
                serviceOrders.Clear();

                foreach (var order in orders)
                {
                    allServiceOrders.Add(order);
                    serviceOrders.Add(order);
                }

                // Initialize grouped orders
                UpdateGroupedOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load data from database: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
