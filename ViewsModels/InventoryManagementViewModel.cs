using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using gentech_services.MVVM;
using gentech_services.Models;
using gentech_services.Services;
using gentech_services.Data;
using gentech_services.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace gentech_services.ViewsModels
{
    internal class InventoryManagementViewModel : ViewModelBase
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly InventoryLogService _inventoryLogService;

        private ObservableCollection<ProductViewModel> products;
        private ObservableCollection<ProductViewModel> allProducts;
        private ProductViewModel selectedProduct;
        private string searchText;

        // Low stock products
        private ObservableCollection<ProductViewModel> lowStockProducts;
        private bool hasLowStockItems;

        // Add Product Form Properties
        private string productName;
        private string selectedCategory;
        private decimal? price;
        private int? lowStockLevel;
        private string description;

        // Edit Product Form Properties
        private string editProductName;
        private string editProductCode;
        private string editSelectedCategory;
        private decimal? editPrice;
        private int? editLowStockLevel;
        private string editDescription;

        // View Product Properties
        private string viewProductCode;
        private string viewCategory;
        private string viewStatus;
        private string viewPrice;
        private string viewStock;
        private int? viewLowStockLevel;
        private string viewDescription;

        // Modal Visibility
        private bool isAddModalVisible;
        private bool isEditModalVisible;
        private bool isViewModalVisible;
        private bool isStockInModalVisible;
        private bool isStockOutModalVisible;

        // Stock In Modal Properties
        private string stockInProductName;
        private int? stockInQuantity;
        private string stockInReason;
        private int stockInCurrentStock;

        // Stock Out Modal Properties
        private string stockOutProductName;
        private int? stockOutQuantity;
        private string stockOutReason;
        private int stockOutCurrentStock;

        public ObservableCollection<ProductViewModel> Products
        {
            get { return products; }
            set
            {
                products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        public ProductViewModel SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }

        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ApplySearch();
            }
        }

        public ObservableCollection<ProductViewModel> LowStockProducts
        {
            get { return lowStockProducts; }
            set
            {
                lowStockProducts = value;
                OnPropertyChanged(nameof(LowStockProducts));
            }
        }

        public bool HasLowStockItems
        {
            get { return hasLowStockItems; }
            set
            {
                hasLowStockItems = value;
                OnPropertyChanged(nameof(HasLowStockItems));
            }
        }

        // Add Product Form Properties
        public string ProductName
        {
            get { return productName; }
            set
            {
                productName = value;
                OnPropertyChanged(nameof(ProductName));
                AddProductCommand.RaiseCanExecuteChanged();
            }
        }

        public int? LowStockLevel
        {
            get { return lowStockLevel; }
            set
            {
                lowStockLevel = value;
                OnPropertyChanged(nameof(LowStockLevel));
                AddProductCommand.RaiseCanExecuteChanged();
            }
        }

        public string SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                AddProductCommand.RaiseCanExecuteChanged();
            }
        }

        public decimal? Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged(nameof(Price));
                AddProductCommand.RaiseCanExecuteChanged();
            }
        }
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        // Edit Product Form Properties
        public string EditProductName
        {
            get { return editProductName; }
            set
            {
                editProductName = value;
                OnPropertyChanged(nameof(EditProductName));
                SaveEditCommand.RaiseCanExecuteChanged();
            }
        }

        public string EditProductCode
        {
            get { return editProductCode; }
            set
            {
                editProductCode = value;
                OnPropertyChanged(nameof(EditProductCode));
            }
        }

        public int? EditLowStockLevel
        {
            get { return editLowStockLevel; }
            set
            {
                editLowStockLevel = value;
                OnPropertyChanged(nameof(EditLowStockLevel));
                SaveEditCommand.RaiseCanExecuteChanged();
            }
        }

        public string EditSelectedCategory
        {
            get { return editSelectedCategory; }
            set
            {
                editSelectedCategory = value;
                OnPropertyChanged(nameof(EditSelectedCategory));
            }
        }

        public decimal? EditPrice
        {
            get { return editPrice; }
            set
            {
                editPrice = value;
                OnPropertyChanged(nameof(EditPrice));
                SaveEditCommand.RaiseCanExecuteChanged();
            }
        }

        public string EditDescription
        {
            get { return editDescription; }
            set
            {
                editDescription = value;
                OnPropertyChanged(nameof(EditDescription));
            }
        }

        // View Product Properties
        public string ViewProductCode
        {
            get { return viewProductCode; }
            set
            {
                viewProductCode = value;
                OnPropertyChanged(nameof(ViewProductCode));
            }
        }

        public string ViewCategory
        {
            get { return viewCategory; }
            set
            {
                viewCategory = value;
                OnPropertyChanged(nameof(ViewCategory));
            }
        }

        public string ViewStatus
        {
            get { return viewStatus; }
            set
            {
                viewStatus = value;
                OnPropertyChanged(nameof(ViewStatus));
            }
        }

        public string ViewPrice
        {
            get { return viewPrice; }
            set
            {
                viewPrice = value;
                OnPropertyChanged(nameof(ViewPrice));
            }
        }

        public string ViewStock
        {
            get { return viewStock; }
            set
            {
                viewStock = value;
                OnPropertyChanged(nameof(ViewStock));
            }
        }

        public string ViewLowStockLevel
        {
            get { return viewLowStockLevel?.ToString() ?? "0"; }
            set
            {
                if (int.TryParse(value, out int level))
                {
                    viewLowStockLevel = level;
                }
                OnPropertyChanged(nameof(ViewLowStockLevel));
            }
        }

        public string ViewDescription
        {
            get { return viewDescription; }
            set
            {
                viewDescription = value;
                OnPropertyChanged(nameof(ViewDescription));
            }
        }

        // Modal Visibility
        public bool IsAddModalVisible
        {
            get { return isAddModalVisible; }
            set
            {
                isAddModalVisible = value;
                OnPropertyChanged(nameof(IsAddModalVisible));
            }
        }

        public bool IsEditModalVisible
        {
            get { return isEditModalVisible; }
            set
            {
                isEditModalVisible = value;
                OnPropertyChanged(nameof(IsEditModalVisible));
            }
        }

        public bool IsViewModalVisible
        {
            get { return isViewModalVisible; }
            set
            {
                isViewModalVisible = value;
                OnPropertyChanged(nameof(IsViewModalVisible));
            }
        }

        public bool IsStockInModalVisible
        {
            get { return isStockInModalVisible; }
            set
            {
                isStockInModalVisible = value;
                OnPropertyChanged(nameof(IsStockInModalVisible));
            }
        }

        public string StockInProductName
        {
            get { return stockInProductName; }
            set
            {
                stockInProductName = value;
                OnPropertyChanged(nameof(StockInProductName));
            }
        }

        public int? StockInQuantity
        {
            get { return stockInQuantity; }
            set
            {
                stockInQuantity = value;
                OnPropertyChanged(nameof(StockInQuantity));
                AddStockCommand.RaiseCanExecuteChanged();
            }
        }

        public string StockInReason
        {
            get { return stockInReason; }
            set
            {
                stockInReason = value;
                OnPropertyChanged(nameof(StockInReason));
            }
        }

        public int StockInCurrentStock
        {
            get { return stockInCurrentStock; }
            set
            {
                stockInCurrentStock = value;
                OnPropertyChanged(nameof(StockInCurrentStock));
            }
        }

        public bool IsStockOutModalVisible
        {
            get { return isStockOutModalVisible; }
            set
            {
                isStockOutModalVisible = value;
                OnPropertyChanged(nameof(IsStockOutModalVisible));
            }
        }

        public string StockOutProductName
        {
            get { return stockOutProductName; }
            set
            {
                stockOutProductName = value;
                OnPropertyChanged(nameof(StockOutProductName));
            }
        }

        public int? StockOutQuantity
        {
            get { return stockOutQuantity; }
            set
            {
                stockOutQuantity = value;
                OnPropertyChanged(nameof(StockOutQuantity));
                RemoveStockCommand.RaiseCanExecuteChanged();
            }
        }

        public string StockOutReason
        {
            get { return stockOutReason; }
            set
            {
                stockOutReason = value;
                OnPropertyChanged(nameof(StockOutReason));
            }
        }

        public int StockOutCurrentStock
        {
            get { return stockOutCurrentStock; }
            set
            {
                stockOutCurrentStock = value;
                OnPropertyChanged(nameof(StockOutCurrentStock));
            }
        }

        // Commands
        public RelayCommand NewProductCommand { get; private set; }
        public RelayCommand AddProductCommand { get; private set; }
        public RelayCommand CancelAddCommand { get; private set; }
        public RelayCommand EditProductCommand { get; private set; }
        public RelayCommand SaveEditCommand { get; private set; }
        public RelayCommand ClearEditCommand { get; private set; }
        public RelayCommand CloseEditModalCommand { get; private set; }
        public RelayCommand ViewProductCommand { get; private set; }
        public RelayCommand CloseViewModalCommand { get; private set; }
        public RelayCommand StockInCommand { get; private set; }
        public RelayCommand StockOutCommand { get; private set; }
        public RelayCommand AddStockCommand { get; private set; }
        public RelayCommand CancelStockInCommand { get; private set; }
        public RelayCommand RemoveStockCommand { get; private set; }
        public RelayCommand CancelStockOutCommand { get; private set; }
        public RelayCommand DeleteProductCommand { get; private set; }

        public InventoryManagementViewModel()
        {
            // Initialize services
            var optionsBuilder = new DbContextOptionsBuilder<GentechDbContext>();
            optionsBuilder.UseSqlite("Data Source=gentech.db");
            var context = new GentechDbContext(optionsBuilder.Options);

            var categoryRepository = new CategoryRepository(context);
            var productRepository = new ProductRepository(context);
            var inventoryLogRepository = new InventoryLogRepository(context);

            _categoryService = new CategoryService(categoryRepository);
            _inventoryLogService = new InventoryLogService(inventoryLogRepository);
            _productService = new ProductService(productRepository, _inventoryLogService);

            // Initialize commands
            NewProductCommand = new RelayCommand(obj => OpenAddModal());
            AddProductCommand = new RelayCommand(async obj => await AddProduct(), obj => CanAddProduct());
            CancelAddCommand = new RelayCommand(obj => CloseAddModal());
            EditProductCommand = new RelayCommand(obj => OpenEditModal(obj as ProductViewModel));
            SaveEditCommand = new RelayCommand(async obj => await SaveEdit(), obj => CanSaveEdit());
            ClearEditCommand = new RelayCommand(obj => ClearEditForm());
            CloseEditModalCommand = new RelayCommand(obj => CloseEditModal());
            ViewProductCommand = new RelayCommand(obj => OpenViewModal(obj as ProductViewModel));
            CloseViewModalCommand = new RelayCommand(obj => CloseViewModal());
            StockInCommand = new RelayCommand(obj => OpenStockInModal());
            StockOutCommand = new RelayCommand(obj => OpenStockOutModal());
            AddStockCommand = new RelayCommand(async obj => await AddStock(), obj => CanAddStock());
            CancelStockInCommand = new RelayCommand(obj => CloseStockInModal());
            RemoveStockCommand = new RelayCommand(async obj => await RemoveStock(), obj => CanRemoveStock());
            CancelStockOutCommand = new RelayCommand(obj => CloseStockOutModal());
            DeleteProductCommand = new RelayCommand(async obj => await DeleteProduct(obj as ProductViewModel));

            // Load data from database
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var dbProducts = await _productService.GetActiveProductsAsync();

                allProducts = new ObservableCollection<ProductViewModel>(
                    dbProducts.Select(p => new ProductViewModel
                    {
                        ProductID = p.ProductID,
                        Name = p.Name,
                        ProductCode = p.SKU,
                        CategoryName = p.Category?.Name ?? "Unknown",
                        Price = p.Price,
                        StockQuanity = p.StockQuantity,
                        LowStockLevel = p.LowStockLevel,
                        IsActive = p.IsActive,
                        Description = p.Description ?? string.Empty,
                        CreatedAt = p.CreatedAt
                    })
                );

                Products = new ObservableCollection<ProductViewModel>(allProducts);
                UpdateLowStockAlert();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateLowStockAlert()
        {
            var lowStock = allProducts.Where(p => p.StockQuanity <= p.LowStockLevel).ToList();
            LowStockProducts = new ObservableCollection<ProductViewModel>(lowStock);
            HasLowStockItems = lowStock.Any();
        }

        private void ApplySearch()
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                Products = new ObservableCollection<ProductViewModel>(allProducts);
            }
            else
            {
                string search = searchText.ToLower().Trim();
                var filtered = allProducts.Where(p =>
                    p.ProductID.ToString().Contains(search) ||
                    p.Name.ToLower().Contains(search) ||
                    (p.ProductCode != null && p.ProductCode.ToLower().Contains(search)) ||
                    p.CategoryName.ToLower().Contains(search)
                ).ToList();

                Products = new ObservableCollection<ProductViewModel>(filtered);
            }
        }

        private void OpenAddModal()
        {
            IsAddModalVisible = true;
        }

        private void CloseAddModal()
        {
            IsAddModalVisible = false;
            ClearAddForm();
        }

        private void ClearAddForm()
        {
            ProductName = string.Empty;
            SelectedCategory = null;
            Price = null;
            LowStockLevel = null;
            Description = string.Empty;
        }

        private string GenerateProductCode()
        {
            // Format: PYYMM XXX (e.g., P2512001)
            DateTime now = DateTime.Now;
            string yearMonth = now.ToString("yyMM");

            string prefix = $"P{yearMonth}";
            int maxSeq = 0;

            foreach (var product in allProducts)
            {
                if (product.ProductCode != null && product.ProductCode.StartsWith(prefix))
                {
                    string seqPart = product.ProductCode.Substring(prefix.Length);
                    if (int.TryParse(seqPart, out int seq))
                    {
                        if (seq > maxSeq) maxSeq = seq;
                    }
                }
            }

            int nextSeq = maxSeq + 1;
            return $"{prefix}{nextSeq:000}";
        }

        private bool CanAddProduct()
        {
            return !string.IsNullOrWhiteSpace(ProductName) &&
                   !string.IsNullOrWhiteSpace(SelectedCategory) &&
                   Price.HasValue && Price.Value > 0 &&
                   LowStockLevel.HasValue && LowStockLevel.Value >= 0;
        }

        private async Task AddProduct()
        {
            try
            {
                var currentUser = AuthenticationService.Instance.CurrentUser;
                if (currentUser == null)
                {
                    MessageBox.Show("User not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get category by name
                var categories = await _categoryService.GetProductCategoriesAsync();
                var category = categories.FirstOrDefault(c => c.Name == SelectedCategory);

                if (category == null)
                {
                    MessageBox.Show("Selected category not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string sku = GenerateProductCode();

                var product = await _productService.CreateProductAsync(
                    ProductName.Trim(),
                    Description?.Trim(),
                    Price.Value,
                    sku,
                    0, // Initial stock
                    LowStockLevel.Value,
                    category.CategoryID,
                    currentUser.UserID
                );

                var newProduct = new ProductViewModel
                {
                    ProductID = product.ProductID,
                    Name = product.Name,
                    ProductCode = product.SKU,
                    CategoryName = category.Name,
                    Price = product.Price,
                    StockQuanity = product.StockQuantity,
                    LowStockLevel = product.LowStockLevel,
                    IsActive = product.IsActive,
                    Description = product.Description ?? string.Empty,
                    CreatedAt = product.CreatedAt
                };

                allProducts.Add(newProduct);
                Products.Add(newProduct);
                UpdateLowStockAlert();

                MessageBox.Show($"Product '{newProduct.Name}' has been added successfully.\nProduct Code: {newProduct.ProductCode}",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CloseAddModal();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenEditModal(ProductViewModel product)
        {
            if (product == null) return;

            SelectedProduct = product;

            EditProductName = product.Name;
            EditProductCode = product.ProductCode;
            EditSelectedCategory = product.CategoryName;
            EditPrice = product.Price;
            EditLowStockLevel = product.LowStockLevel;
            EditDescription = product.Description;

            IsEditModalVisible = true;
        }

        private void ClearEditForm()
        {
            EditProductName = string.Empty;
            EditProductCode = string.Empty;
            EditSelectedCategory = null;
            EditPrice = null;
            EditLowStockLevel = null;
            EditDescription = string.Empty;
        }

        private bool CanSaveEdit()
        {
            return !string.IsNullOrWhiteSpace(EditProductName) &&
                   EditPrice.HasValue && EditPrice.Value > 0 &&
                   EditLowStockLevel.HasValue && EditLowStockLevel.Value >= 0;
        }

        private async Task SaveEdit()
        {
            try
            {
                if (SelectedProduct == null) return;

                // Get category by name
                var categories = await _categoryService.GetProductCategoriesAsync();
                var category = categories.FirstOrDefault(c => c.Name == EditSelectedCategory);

                if (category == null)
                {
                    MessageBox.Show("Selected category not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updatedProduct = await _productService.UpdateProductAsync(
                    SelectedProduct.ProductID,
                    EditProductName.Trim(),
                    EditDescription?.Trim(),
                    EditPrice.Value,
                    SelectedProduct.ProductCode,
                    EditLowStockLevel.Value,
                    category.CategoryID
                );

                SelectedProduct.Name = updatedProduct.Name;
                SelectedProduct.Price = updatedProduct.Price;
                SelectedProduct.LowStockLevel = updatedProduct.LowStockLevel;
                SelectedProduct.Description = updatedProduct.Description ?? string.Empty;
                SelectedProduct.CategoryName = category.Name;

                var temp = Products;
                Products = null;
                Products = temp;

                UpdateLowStockAlert();

                MessageBox.Show($"Product '{SelectedProduct.Name}' has been updated successfully.",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                IsEditModalVisible = false;
                ClearEditForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenViewModal(ProductViewModel product)
        {
            if (product == null) return;

            ViewProductCode = product.ProductCode;
            ViewCategory = product.CategoryName;
            ViewStatus = product.StockQuanity <= product.LowStockLevel ? "Low Stock" : "On Stock";
            ViewPrice = $"₱{product.Price:N0}";
            ViewStock = product.StockQuanity.ToString();
            ViewLowStockLevel = product.LowStockLevel.ToString();
            ViewDescription = product.Description;

            IsViewModalVisible = true;
        }

        private void CloseViewModal()
        {
            IsViewModalVisible = false;
        }

        private void CloseEditModal()
        {
            IsEditModalVisible = false;
        }

        private void OpenStockInModal()
        {
            if (SelectedProduct == null) return;

            StockInProductName = !string.IsNullOrWhiteSpace(EditProductName) ? EditProductName : SelectedProduct.Name;
            StockInCurrentStock = SelectedProduct.StockQuanity;
            StockInQuantity = null;
            StockInReason = string.Empty;
            IsStockInModalVisible = true;
        }

        private void CloseStockInModal()
        {
            IsStockInModalVisible = false;
            StockInProductName = string.Empty;
            StockInQuantity = null;
            StockInReason = string.Empty;
        }

        private bool CanAddStock()
        {
            return StockInQuantity.HasValue && StockInQuantity.Value > 0;
        }

        private async Task AddStock()
        {
            try
            {
                if (SelectedProduct == null || !StockInQuantity.HasValue) return;

                var currentUser = AuthenticationService.Instance.CurrentUser;
                if (currentUser == null)
                {
                    MessageBox.Show("User not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updatedProduct = await _productService.StockInAsync(
                    SelectedProduct.ProductID,
                    StockInQuantity.Value,
                    currentUser.UserID,
                    StockInReason
                );

                SelectedProduct.StockQuanity = updatedProduct.StockQuantity;

                var temp = Products;
                Products = null;
                Products = temp;

                UpdateLowStockAlert();

                MessageBox.Show($"Added {StockInQuantity.Value} units to '{SelectedProduct.Name}'. New stock: {SelectedProduct.StockQuanity}",
                    "Stock Added", MessageBoxButton.OK, MessageBoxImage.Information);

                CloseStockInModal();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add stock: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenStockOutModal()
        {
            if (SelectedProduct == null) return;

            StockOutProductName = !string.IsNullOrWhiteSpace(EditProductName) ? EditProductName : SelectedProduct.Name;
            StockOutCurrentStock = SelectedProduct.StockQuanity;
            StockOutQuantity = null;
            StockOutReason = string.Empty;
            IsStockOutModalVisible = true;
        }

        private void CloseStockOutModal()
        {
            IsStockOutModalVisible = false;
            StockOutProductName = string.Empty;
            StockOutQuantity = null;
            StockOutReason = string.Empty;
        }

        private bool CanRemoveStock()
        {
            return StockOutQuantity.HasValue && StockOutQuantity.Value > 0;
        }

        private async Task RemoveStock()
        {
            try
            {
                if (SelectedProduct == null || !StockOutQuantity.HasValue) return;

                var currentUser = AuthenticationService.Instance.CurrentUser;
                if (currentUser == null)
                {
                    MessageBox.Show("User not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updatedProduct = await _productService.StockOutAsync(
                    SelectedProduct.ProductID,
                    StockOutQuantity.Value,
                    currentUser.UserID,
                    StockOutReason
                );

                SelectedProduct.StockQuanity = updatedProduct.StockQuantity;

                var temp = Products;
                Products = null;
                Products = temp;

                UpdateLowStockAlert();

                MessageBox.Show($"Removed {StockOutQuantity.Value} units from '{SelectedProduct.Name}'. New stock: {SelectedProduct.StockQuanity}",
                    "Stock Removed", MessageBoxButton.OK, MessageBoxImage.Information);

                CloseStockOutModal();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to remove stock: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteProduct(ProductViewModel product)
        {
            try
            {
                if (product == null) return;

                if (product.StockQuanity > 0)
                {
                    MessageBox.Show($"Cannot delete '{product.Name}'. Product has {product.StockQuanity} units in stock.\nPlease remove all stock before deleting.",
                        "Cannot Delete Product", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Are you sure you want to delete '{product.Name}'?\nThis action cannot be undone.",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _productService.DeleteProductAsync(product.ProductID);

                    allProducts.Remove(product);
                    Products.Remove(product);

                    MessageBox.Show($"Product '{product.Name}' has been deleted successfully.",
                        "Product Deleted", MessageBoxButton.OK, MessageBoxImage.Information);

                    UpdateLowStockAlert();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // ViewModel for Product display
    public class ProductViewModel : INotifyPropertyChanged
    {
        private int productID;
        private string name;
        private string productCode;
        private string categoryName;
        private decimal price;
        private int stockQuanity;
        private int lowStockLevel;
        private bool isActive;
        private string description;
        private DateTime createdAt;

        public int ProductID
        {
            get { return productID; }
            set
            {
                productID = value;
                OnPropertyChanged(nameof(ProductID));
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string ProductCode
        {
            get { return productCode; }
            set
            {
                productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }

        public int LowStockLevel
        {
            get { return lowStockLevel; }
            set
            {
                lowStockLevel = value;
                OnPropertyChanged(nameof(LowStockLevel));
            }
        }

        public string CategoryName
        {
            get { return categoryName; }
            set
            {
                categoryName = value;
                OnPropertyChanged(nameof(CategoryName));
            }
        }

        public decimal Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        public int StockQuanity
        {
            get { return stockQuanity; }
            set
            {
                stockQuanity = value;
                OnPropertyChanged(nameof(StockQuanity));
            }
        }

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public DateTime CreatedAt
        {
            get { return createdAt; }
            set
            {
                createdAt = value;
                OnPropertyChanged(nameof(CreatedAt));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
