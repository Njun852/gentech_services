using gentech_services.Models;
using ProductServicesManagementSystem.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace gentech_services.Views.UserControls
{
    public partial class EditAppointmentModal : UserControl
    {
        private ServiceOrder currentOrder;
        public Action<ServiceOrder> OnSaveChanges { get; set; }

        public EditAppointmentModal()
        {
            InitializeComponent();
        }

        public void ShowModal(ServiceOrder order)
        {
            if (order == null) return;

            currentOrder = order;

            // Set order ID
            OrderIdText.Text = $"#S{order.SaleID:000}";

            // Set customer information
            if (order.Customer != null)
            {
                CustomerNameTextBox.Text = $"{order.Customer.FirstName} {order.Customer.LastName}";
                EmailTextBox.Text = order.Customer.Email ?? "";
                PhoneTextBox.Text = order.Customer.Phone ?? "";
            }

            // Set appointment date
            AppointmentDatePicker.SelectedDate = order.AppointmentDate;

            // Set issue description
            IssueDescriptionTextBox.Text = order.Service?.Description ?? "";

            // Show the modal
            ModalOverlay.Visibility = Visibility.Visible;
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (currentOrder == null) return;

            // Validate all fields
            if (!ValidateForm())
            {
                return;
            }

            // Update customer information
            if (currentOrder.Customer != null)
            {
                var fullName = CustomerNameTextBox.Text.Trim().Split(' ');
                if (fullName.Length >= 2)
                {
                    currentOrder.Customer.FirstName = fullName[0];
                    currentOrder.Customer.LastName = string.Join(" ", fullName.Skip(1));
                }
                else if (fullName.Length == 1)
                {
                    currentOrder.Customer.FirstName = fullName[0];
                }

                currentOrder.Customer.Email = EmailTextBox.Text.Trim();
                currentOrder.Customer.Phone = PhoneTextBox.Text.Trim();
            }

            // Update appointment date
            if (AppointmentDatePicker.SelectedDate.HasValue)
            {
                currentOrder.AppointmentDate = AppointmentDatePicker.SelectedDate.Value;
            }

            // Update issue description
            if (currentOrder.Service != null)
            {
                currentOrder.Service.Description = IssueDescriptionTextBox.Text;
            }

            // Notify parent
            OnSaveChanges?.Invoke(currentOrder);

            MessageBox.Show("Appointment information updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            CloseModal();
        }

        private bool ValidateForm()
        {
            // Validate customer name
            if (string.IsNullOrWhiteSpace(CustomerNameTextBox.Text))
            {
                MessageBox.Show("Customer name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                CustomerNameTextBox.Focus();
                return false;
            }

            // Validate email
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Email is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return false;
            }

            if (!IsValidEmail(EmailTextBox.Text.Trim()))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return false;
            }

            // Validate phone
            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("Phone number is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PhoneTextBox.Focus();
                return false;
            }

            if (!IsValidPhone(PhoneTextBox.Text.Trim()))
            {
                MessageBox.Show("Please enter a valid phone number (10-11 digits).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PhoneTextBox.Focus();
                return false;
            }

            // Validate appointment date
            if (!AppointmentDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Appointment date is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                AppointmentDatePicker.Focus();
                return false;
            }

            return true;
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseModal();
        }

        public void CloseModal()
        {
            ModalOverlay.Visibility = Visibility.Collapsed;
            currentOrder = null;
        }
    }
}
