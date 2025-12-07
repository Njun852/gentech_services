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

            // Description - using issue description from appointment
            DescriptionText.Text = order.IssueDescription ?? "No description available";

            // Show the modal
            ModalOverlay.Visibility = Visibility.Visible;
        }

        public void ShowModal(System.Collections.Generic.List<ServiceOrder> orders)
        {
            if (orders == null || orders.Count == 0) return;

            var order = orders[0]; // In new schema, single service order contains multiple service items

            // Set order details
            OrderIdText.Text = $"S{order.ServiceOrderID:000}";
            CreatedDateText.Text = $"Created: {order.CreatedAt:MM/dd/yyyy}";

            // Set status
            StatusText.Text = order.Status;
            SetStatusBadgeColor(order.Status);

            // Customer details
            CustomerNameText.Text = order.FullName ?? "N/A";
            CustomerEmailText.Text = order.Email ?? "N/A";
            CustomerPhoneText.Text = order.Phone ?? "N/A";

            // Clear existing services
            ServiceOrdersList.Children.Clear();

            // Loop through all service items and add each service
            decimal totalCost = 0;
            if (order.ServiceOrderItems != null)
            {
                foreach (var item in order.ServiceOrderItems)
                {
                    if (item.Service != null)
                    {
                        // Create a horizontal stack panel for status badge + service name
                        var serviceContainer = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(0, 0, 0, 6)
                        };

                        // Status badge (all items share the same order status)
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
                            Text = item.Service.Name ?? "N/A",
                            FontSize = 13,
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        serviceContainer.Children.Add(statusBorder);
                        serviceContainer.Children.Add(serviceText);
                        ServiceOrdersList.Children.Add(serviceContainer);

                        totalCost += item.TotalPrice;
                    }
                }
            }

            CostText.Text = $"₱ {totalCost:N0}";

            // Technician - not yet implemented in new schema
            TechnicianText.Text = "Unassigned";

            // Description
            DescriptionText.Text = order.IssueNotes ?? "No description available";

            // Show the modal
            ModalOverlay.Visibility = Visibility.Visible;
        }

        private string GetOverallStatus(System.Collections.Generic.List<ServiceOrder> orders)
        {
            if (orders == null || orders.Count == 0) return "Pending";

            // Priority 1: If ANY service is Pending
            if (orders.Any(o => o.Status?.ToLower() == "pending"))
                return "Pending";

            // Priority 2: If ANY service is In Progress
            if (orders.Any(o => o.Status?.ToLower() == "in progress"))
                return "In Progress";

            // Priority 3: If ALL services are Completed
            if (orders.All(o => o.Status?.ToLower() == "completed"))
                return "Completed";

            // Priority 4: If ALL services are Cancelled
            if (orders.All(o => o.Status?.ToLower() == "cancelled"))
                return "Cancelled";

            // Default fallback
            return orders.First()?.Status ?? "Pending";
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
                case "in progress":
                    StatusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5F0FF"));
                    StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3366CC"));
                    break;
                default:
                    StatusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"));
                    StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
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
                case "in progress":
                    border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5F0FF"));
                    textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3366CC"));
                    break;
                default:
                    border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"));
                    textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
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
