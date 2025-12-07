using gentech_services.Models;
using ProductServicesManagementSystem.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            OrderIdText.Text = $"S{order.ServiceOrderID:000}";

            // Set customer information
            CustomerNameTextBox.Text = order.FullName ?? "";
            EmailTextBox.Text = order.Email ?? "";
            PhoneTextBox.Text = order.Phone ?? "";

            // Set appointment date
            AppointmentDatePicker.SelectedDate = order.ScheduledAt;

            // Set issue description
            IssueDescriptionTextBox.Text = order.IssueNotes ?? "";

            // Clear any validation errors
            ClearAllValidationErrors();

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
            currentOrder.FullName = CustomerNameTextBox.Text.Trim();
            currentOrder.Email = EmailTextBox.Text.Trim();
            currentOrder.Phone = PhoneTextBox.Text.Trim();

            // Update appointment date
            if (AppointmentDatePicker.SelectedDate.HasValue)
            {
                currentOrder.ScheduledAt = AppointmentDatePicker.SelectedDate.Value;
            }

            // Update issue description
            currentOrder.IssueNotes = IssueDescriptionTextBox.Text.Trim();

            // Notify parent to refresh the UI
            OnSaveChanges?.Invoke(currentOrder);

            CloseModal();
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            // Clear all errors first
            ClearAllValidationErrors();

            // Validate customer name
            if (string.IsNullOrWhiteSpace(CustomerNameTextBox.Text))
            {
                ShowValidationError(CustomerNameBorder, CustomerNameError, "Customer name is required");
                isValid = false;
            }

            // Validate email
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ShowValidationError(EmailBorder, EmailError, "Email is required");
                isValid = false;
            }
            else if (!IsValidEmail(EmailTextBox.Text.Trim()))
            {
                ShowValidationError(EmailBorder, EmailError, "Please enter a valid email address");
                isValid = false;
            }

            // Validate phone
            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                ShowValidationError(PhoneBorder, PhoneError, "Phone number is required");
                isValid = false;
            }
            else if (!IsValidPhone(PhoneTextBox.Text.Trim()))
            {
                ShowValidationError(PhoneBorder, PhoneError, "Please enter a valid phone number (10-11 digits)");
                isValid = false;
            }

            // Validate appointment date
            if (!AppointmentDatePicker.SelectedDate.HasValue)
            {
                ShowValidationError(AppointmentDateBorder, AppointmentDateError, "Appointment date is required");
                isValid = false;
            }

            return isValid;
        }

        private void ShowValidationError(System.Windows.Controls.Border border, TextBlock errorLabel, string message)
        {
            border.BorderBrush = new SolidColorBrush(Colors.Red);
            border.BorderThickness = new Thickness(1.5);
            errorLabel.Text = message;
            errorLabel.Visibility = Visibility.Visible;
        }

        private void ClearValidationError(System.Windows.Controls.Border border, TextBlock errorLabel)
        {
            border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDE1E6"));
            border.BorderThickness = new Thickness(1);
            errorLabel.Visibility = Visibility.Collapsed;
        }

        private void ClearAllValidationErrors()
        {
            ClearValidationError(CustomerNameBorder, CustomerNameError);
            ClearValidationError(EmailBorder, EmailError);
            ClearValidationError(PhoneBorder, PhoneError);
            ClearValidationError(AppointmentDateBorder, AppointmentDateError);
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

        private void CustomerNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CustomerNameTextBox.Text))
            {
                ClearValidationError(CustomerNameBorder, CustomerNameError);
            }
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(EmailTextBox.Text) && IsValidEmail(EmailTextBox.Text.Trim()))
            {
                ClearValidationError(EmailBorder, EmailError);
            }
        }

        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PhoneTextBox.Text) && IsValidPhone(PhoneTextBox.Text.Trim()))
            {
                ClearValidationError(PhoneBorder, PhoneError);
            }
        }

        private void AppointmentDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppointmentDatePicker.SelectedDate.HasValue)
            {
                ClearValidationError(AppointmentDateBorder, AppointmentDateError);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseModal();
        }

        public void CloseModal()
        {
            ModalOverlay.Visibility = Visibility.Collapsed;
            currentOrder = null;
            ClearAllValidationErrors();
        }
    }
}
