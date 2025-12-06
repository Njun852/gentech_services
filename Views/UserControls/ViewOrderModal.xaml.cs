using gentech_services.Models;
using ProductServicesManagementSystem.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace gentech_services.Views.UserControls
{
    public partial class ViewOrderModal : UserControl
    {
        public ViewOrderModal()
        {
            InitializeComponent();
        }

        public void ShowModal(ServiceOrder order)
        {
            if (order == null) return;

            // Set order details
            OrderIdText.Text = $"#S{order.SaleID:000}";
            CreatedDateText.Text = $"Created: {order.AppointmentDate:MM/dd/yyyy}";

            // Set status badge
            StatusText.Text = order.Status;
            SetStatusBadgeColor(order.Status);

            // Customer details
            if (order.Customer != null)
            {
                CustomerNameText.Text = $"{order.Customer.FirstName} {order.Customer.LastName}";
                CustomerEmailText.Text = order.Customer.Email ?? "N/A";
                CustomerPhoneText.Text = order.Customer.Phone ?? "N/A";
            }

            // Clear existing services
            ServiceOrdersList.Children.Clear();

            // Service details - for now showing single service, can be extended to multiple services
            if (order.Service != null)
            {
                // Create a horizontal stack panel for status badge + service name
                var serviceContainer = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 0, 0, 6)
                };

                // Status badge
                var statusBorder = new Border
                {
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(8, 3, 8, 3),
                    Margin = new Thickness(0, 0, 8, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };

                var statusTextBlock = new TextBlock
                {
                    Text = order.Status ?? "Pending",
                    FontSize = 11,
                    FontWeight = FontWeights.SemiBold
                };

                // Set status badge colors
                SetServiceStatusColors(statusBorder, statusTextBlock, order.Status);

                statusBorder.Child = statusTextBlock;

                // Service name
                var serviceText = new TextBlock
                {
                    Text = order.Service.Name ?? "N/A",
                    FontSize = 13,
                    VerticalAlignment = VerticalAlignment.Center
                };

                serviceContainer.Children.Add(statusBorder);
                serviceContainer.Children.Add(serviceText);
                ServiceOrdersList.Children.Add(serviceContainer);

                CostText.Text = $"₱ {order.Service.Price:N0}";
            }
            else
            {
                CostText.Text = "₱ 0";
            }

            // Technician
            if (order.Technician != null)
            {
                TechnicianText.Text = order.Technician.Name ?? "Unassigned";
            }
            else
            {
                TechnicianText.Text = "Unassigned";
            }

            // Description - using service description
            DescriptionText.Text = order.Service?.Description ?? "No description available";

            // Show the modal
            ModalOverlay.Visibility = Visibility.Visible;
        }

        private void SetStatusBadgeColor(string status)
        {
            switch (status?.ToLower())
            {
                case "completed":
                    StatusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5F8E5"));
                    StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#44AA44"));
                    break;
                case "pending":
                    StatusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF4E5"));
                    StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8800"));
                    break;
                case "cancelled":
                    StatusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"));
                    StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
                    break;
                case "ongoing":
                default:
                    StatusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE5E5"));
                    StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4444"));
                    break;
            }
        }

        private void SetServiceStatusColors(Border border, TextBlock textBlock, string status)
        {
            switch (status?.ToLower())
            {
                case "completed":
                    border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5F8E5"));
                    textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#44AA44"));
                    break;
                case "pending":
                    border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF4E5"));
                    textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8800"));
                    break;
                case "cancelled":
                    border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"));
                    textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
                    break;
                case "ongoing":
                default:
                    border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE5E5"));
                    textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4444"));
                    break;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ModalOverlay.Visibility = Visibility.Collapsed;
        }

        // Public method to close modal programmatically
        public void CloseModal()
        {
            ModalOverlay.Visibility = Visibility.Collapsed;
        }
    }
}
