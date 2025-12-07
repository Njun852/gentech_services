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

            // Calculate and set overall status based on service items
            string overallStatus = CalculateOverallStatus(order);
            StatusText.Text = overallStatus;
            SetStatusBadgeColor(overallStatus);

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

                        // Status badge for individual service item
                        var statusBorder = new Border
                        {
                            CornerRadius = new CornerRadius(10),
                            Padding = new Thickness(8, 3, 8, 3),
                            Margin = new Thickness(0, 0, 8, 0),
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        var statusTextBlock = new TextBlock
                        {
                            Text = item.Status ?? "Pending",
                            FontSize = 11,
                            FontWeight = FontWeights.SemiBold
                        };

                        // Set status badge colors for this specific item
                        SetServiceStatusColors(statusBorder, statusTextBlock, item.Status);

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

        private string CalculateOverallStatus(ServiceOrder order)
        {
            if (order?.ServiceOrderItems == null || !order.ServiceOrderItems.Any())
            {
                return order?.Status ?? "Pending";
            }

            // Get all service item statuses
            var itemStatuses = order.ServiceOrderItems.Select(item => item.Status).ToList();

            // Apply complex status logic:
            // 1. If at least one is ongoing → Ongoing
            if (itemStatuses.Any(s => s?.ToLower() == "ongoing"))
            {
                return "Ongoing";
            }

            // 2. All services completed → Completed
            if (itemStatuses.All(s => s?.ToLower() == "completed"))
            {
                return "Completed";
            }

            // 3. All services cancelled → Cancelled
            if (itemStatuses.All(s => s?.ToLower() == "cancelled"))
            {
                return "Cancelled";
            }

            // 4. Mix of completed and cancelled only → Completed
            if (itemStatuses.All(s => s?.ToLower() == "completed" || s?.ToLower() == "cancelled") &&
                itemStatuses.Any(s => s?.ToLower() == "completed"))
            {
                return "Completed";
            }

            // 5. All services pending → Pending
            if (itemStatuses.All(s => s?.ToLower() == "pending"))
            {
                return "Pending";
            }

            // Default fallback
            return order.Status ?? "Pending";
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
                case "ongoing":
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
