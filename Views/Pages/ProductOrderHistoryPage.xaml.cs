using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using gentech_services.ViewsModels;
using Microsoft.EntityFrameworkCore;

namespace gentech_services.Views.Pages
{
    public partial class ProductOrderHistoryPage : UserControl
    {
        private ProductOrderHistoryItem selectedOrder;

        public ProductOrderHistoryPage()
        {
            InitializeComponent();

            // Initialize database services
            var dbContext = new gentech_services.Data.GentechDbContext(
                new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<gentech_services.Data.GentechDbContext>()
                    .UseSqlite("Data Source=gentech.db")
                    .Options);

            var productRepository = new gentech_services.Repositories.ProductRepository(dbContext);
            var productOrderRepository = new gentech_services.Repositories.ProductOrderRepository(dbContext);
            var inventoryLogRepository = new gentech_services.Repositories.InventoryLogRepository(dbContext);

            var inventoryLogService = new gentech_services.Services.InventoryLogService(inventoryLogRepository);
            var productService = new gentech_services.Services.ProductService(productRepository, inventoryLogService);
            var productOrderService = new gentech_services.Services.ProductOrderService(productOrderRepository, productRepository, productService);

            DataContext = new ProductOrderHistoryViewModel(productOrderService);
        }

        private void OrderActionButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                selectedOrder = button.DataContext as ProductOrderHistoryItem;
                OrderActionPopup.PlacementTarget = button;
                OrderActionPopup.IsOpen = true;
            }
        }

        private void ViewOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderActionPopup.IsOpen = false;
            var viewModel = DataContext as ProductOrderHistoryViewModel;
            if (viewModel != null && selectedOrder != null)
            {
                viewModel.ShowOrderDetailsCommand.Execute(selectedOrder);
            }
        }

        private void CloseOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ProductOrderHistoryViewModel;
            if (viewModel != null)
            {
                viewModel.CloseOrderDetailsCommand.Execute(null);
            }
        }

        private void ReturnOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderActionPopup.IsOpen = false;
            var viewModel = DataContext as ProductOrderHistoryViewModel;
            if (viewModel != null && selectedOrder != null)
            {
                viewModel.ShowReturnModalCommand.Execute(selectedOrder);
            }
        }

        private void CloseReturnModal_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ProductOrderHistoryViewModel;
            if (viewModel != null)
            {
                viewModel.CloseReturnModalCommand.Execute(null);
            }
        }

        private void VoidOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderActionPopup.IsOpen = false;
            var viewModel = DataContext as ProductOrderHistoryViewModel;
            if (viewModel != null && selectedOrder != null)
            {
                viewModel.ShowVoidModalCommand.Execute(selectedOrder);
            }
        }

        private void CloseVoidModal_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ProductOrderHistoryViewModel;
            if (viewModel != null)
            {
                viewModel.CloseVoidModalCommand.Execute(null);
            }
        }
    }
}
