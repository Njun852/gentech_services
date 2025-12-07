using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using gentech_services.ViewsModels;
using Microsoft.EntityFrameworkCore;

namespace gentech_services.Views.Pages
{
    public partial class ProductOrdersPage : UserControl
    {
        private ProductOrdersViewModel ViewModel => (ProductOrdersViewModel)DataContext;

        public ProductOrdersPage()
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

            DataContext = new ProductOrdersViewModel(productService, productOrderService);
        }

        private void ProductCard_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var product = border?.DataContext as ProductCardViewModel;

            if (product != null)
            {
                ViewModel.AddToCartCommand.Execute(product);
            }
        }
    }
}
