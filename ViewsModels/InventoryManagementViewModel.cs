using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using gentech_services.MVVM;
using gentech_services.Models;

namespace gentech_services.ViewsModels
{
    internal class InventoryManagementViewModel : ViewModelBase
    {
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
            NewProductCommand = new RelayCommand(obj => OpenAddModal());
            AddProductCommand = new RelayCommand(obj => AddProduct(), obj => CanAddProduct());
            CancelAddCommand = new RelayCommand(obj => CloseAddModal());
            EditProductCommand = new RelayCommand(obj => OpenEditModal(obj as ProductViewModel));
            SaveEditCommand = new RelayCommand(obj => SaveEdit(), obj => CanSaveEdit());
            ClearEditCommand = new RelayCommand(obj => ClearEditForm());
            CloseEditModalCommand = new RelayCommand(obj => CloseEditModal());
            ViewProductCommand = new RelayCommand(obj => OpenViewModal(obj as ProductViewModel));
            CloseViewModalCommand = new RelayCommand(obj => CloseViewModal());
            StockInCommand = new RelayCommand(obj => OpenStockInModal());
            StockOutCommand = new RelayCommand(obj => OpenStockOutModal());
            AddStockCommand = new RelayCommand(obj => AddStock(), obj => CanAddStock());
            CancelStockInCommand = new RelayCommand(obj => CloseStockInModal());
            RemoveStockCommand = new RelayCommand(obj => RemoveStock(), obj => CanRemoveStock());
            CancelStockOutCommand = new RelayCommand(obj => CloseStockOutModal());
            DeleteProductCommand = new RelayCommand(obj => DeleteProduct(obj as ProductViewModel));

            LoadSampleData();
            UpdateLowStockAlert();
        }

        private void LoadSampleData()
        {
            allProducts = new ObservableCollection<ProductViewModel>
            {
                new ProductViewModel
                {
                    ProductID = 1,
                    Name = "LENOVO LEGION 3",
                    ProductCode = "P2512001",
                    CategoryName = "LAPTOP",
                    Price = 49999m,
                    StockQuanity = 300,
                    LowStockLevel = 10,
                    IsActive = true,
                    Description = "High Performing Laptop",
                    CreatedAt = DateTime.Now.AddMonths(-6)
                },
                new ProductViewModel
                {
                    ProductID = 2,
                    Name = "Gaming Chair",
                    ProductCode = "P2512002",
                    CategoryName = "Accessories",
                    Price = 15999m,
                    StockQuanity = 8,
                    LowStockLevel = 5,
                    IsActive = true,
                    Description = "Ergonomic gaming chair with lumbar support",
                    CreatedAt = DateTime.Now.AddMonths(-5)
                },
                new ProductViewModel
                {
                    ProductID = 3,
                    Name = "GTX 7090",
                    ProductCode = "P2512003",
                    CategoryName = "Parts",
                    Price = 89990m,
                    StockQuanity = 5,
                    LowStockLevel = 3,
                    IsActive = true,
                    Description = "High-end graphics card for gaming",
                    CreatedAt = DateTime.Now.AddMonths(-4)
                },
                new ProductViewModel
                {
                    ProductID = 4,
                    Name = "Mechanical Keyboard RGB",
                    ProductCode = "P2512004",
                    CategoryName = "Accessories",
                    Price = 8990m,
                    StockQuanity = 45,
                    LowStockLevel = 20,
                    IsActive = true,
                    Description = "RGB mechanical keyboard with blue switches",
                    CreatedAt = DateTime.Now.AddMonths(-4)
                },
                new ProductViewModel
                {
                    ProductID = 5,
                    Name = "Wireless Gaming Mouse",
                    ProductCode = "P2512005",
                    CategoryName = "Accessories",
                    Price = 3490m,
                    StockQuanity = 60,
                    LowStockLevel = 25,
                    IsActive = true,
                    Description = "Wireless gaming mouse with 16000 DPI",
                    CreatedAt = DateTime.Now.AddMonths(-3)
                },
                new ProductViewModel
                {
                    ProductID = 6,
                    Name = "RAM DDR5 32GB Kit",
                    ProductCode = "P2512006",
                    CategoryName = "Parts",
                    Price = 12990m,
                    StockQuanity = 25,
                    LowStockLevel = 10,
                    IsActive = true,
                    Description = "High-speed DDR5 RAM 32GB (2x16GB) kit",
                    CreatedAt = DateTime.Now.AddMonths(-3)
                },
                new ProductViewModel
                {
                    ProductID = 7,
                    Name = "SSD NVMe 2TB",
                    ProductCode = "P2512007",
                    CategoryName = "Parts",
                    Price = 10490m,
                    StockQuanity = 18,
                    LowStockLevel = 8,
                    IsActive = true,
                    Description = "2TB NVMe SSD with 7000MB/s read speed",
                    CreatedAt = DateTime.Now.AddMonths(-2)
                },
                new ProductViewModel
                {
                    ProductID = 8,
                    Name = "27\" 4K Monitor",
                    ProductCode = "P2512008",
                    CategoryName = "Electronics",
                    Price = 28990m,
                    StockQuanity = 12,
                    LowStockLevel = 5,
                    IsActive = true,
                    Description = "27-inch 4K IPS monitor with 144Hz",
                    CreatedAt = DateTime.Now.AddMonths(-2)
                },
                new ProductViewModel
                {
                    ProductID = 9,
                    Name = "CPU Ryzen 9 7950X",
                    ProductCode = "P2512009",
                    CategoryName = "Parts",
                    Price = 35990m,
                    StockQuanity = 7,
                    LowStockLevel = 4,
                    IsActive = true,
                    Description = "16-core 32-thread flagship processor",
                    CreatedAt = DateTime.Now.AddMonths(-1)
                },
                new ProductViewModel
                {
                    ProductID = 10,
                    Name = "Motherboard X670E",
                    ProductCode = "P2512010",
                    CategoryName = "Parts",
                    Price = 18990m,
                    StockQuanity = 15,
                    LowStockLevel = 8,
                    IsActive = true,
                    Description = "ATX motherboard for AMD Ryzen 7000 series",
                    CreatedAt = DateTime.Now.AddMonths(-1)
                },
                new ProductViewModel
                {
                    ProductID = 11,
                    Name = "Power Supply 850W Gold",
                    ProductCode = "P2512011",
                    CategoryName = "Parts",
                    Price = 7490m,
                    StockQuanity = 22,
                    LowStockLevel = 10,
                    IsActive = true,
                    Description = "850W 80+ Gold certified modular PSU",
                    CreatedAt = DateTime.Now.AddDays(-25)
                },
                new ProductViewModel
                {
                    ProductID = 12,
                    Name = "PC Case Mid Tower",
                    ProductCode = "P2512012",
                    CategoryName = "Accessories",
                    Price = 5990m,
                    StockQuanity = 30,
                    LowStockLevel = 15,
                    IsActive = true,
                    Description = "Mid tower case with tempered glass and RGB",
                    CreatedAt = DateTime.Now.AddDays(-20)
                },
                new ProductViewModel
                {
                    ProductID = 13,
                    Name = "CPU Cooler AIO 360mm",
                    ProductCode = "P2512013",
                    CategoryName = "Parts",
                    Price = 9990m,
                    StockQuanity = 14,
                    LowStockLevel = 6,
                    IsActive = true,
                    Description = "360mm AIO liquid cooler with RGB fans",
                    CreatedAt = DateTime.Now.AddDays(-15)
                },
                new ProductViewModel
                {
                    ProductID = 14,
                    Name = "Webcam 4K",
                    ProductCode = "P2512014",
                    CategoryName = "Accessories",
                    Price = 4990m,
                    StockQuanity = 38,
                    LowStockLevel = 20,
                    IsActive = true,
                    Description = "4K webcam with auto-focus and noise reduction",
                    CreatedAt = DateTime.Now.AddDays(-10)
                },
                new ProductViewModel
                {
                    ProductID = 15,
                    Name = "Headset 7.1 Surround",
                    ProductCode = "P2512015",
                    CategoryName = "Accessories",
                    Price = 6490m,
                    StockQuanity = 42,
                    LowStockLevel = 20,
                    IsActive = true,
                    Description = "7.1 surround sound gaming headset",
                    CreatedAt = DateTime.Now.AddDays(-5)
                }
            };

            Products = new ObservableCollection<ProductViewModel>(allProducts);
        }

        private void UpdateLowStockAlert()
        {
            // Use each product's LowStockLevel threshold instead of hardcoded value
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
            string yearMonth = now.ToString("yyMM"); // e.g., "2512" for December 2025

            // Find the next sequential number for this month
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
            return $"{prefix}{nextSeq:000}"; // e.g., "P2512001"
        }

        private bool CanAddProduct()
        {
            return !string.IsNullOrWhiteSpace(ProductName) &&
                   !string.IsNullOrWhiteSpace(SelectedCategory) &&
                   Price.HasValue && Price.Value > 0 &&
                   LowStockLevel.HasValue && LowStockLevel.Value >= 0;
        }

        private void AddProduct()
        {
            int nextID = allProducts.Count > 0 ? allProducts.Max(p => p.ProductID) + 1 : 1;

            var newProduct = new ProductViewModel
            {
                ProductID = nextID,
                Name = ProductName.Trim(),
                ProductCode = GenerateProductCode(), // Auto-generate product code
                CategoryName = SelectedCategory,
                Price = Price.Value,
                StockQuanity = 0, // Default stock quantity
                LowStockLevel = LowStockLevel.Value,
                IsActive = true,
                Description = Description?.Trim() ?? string.Empty,
                CreatedAt = DateTime.Now
            };

            allProducts.Add(newProduct);
            Products.Add(newProduct);
            UpdateLowStockAlert();

            MessageBox.Show($"Product '{newProduct.Name}' has been added successfully.\nProduct Code: {newProduct.ProductCode}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            CloseAddModal();
        }

        private void OpenEditModal(ProductViewModel product)
        {
            if (product == null) return;

            SelectedProduct = product;

            EditProductName = product.Name;
            EditProductCode = product.ProductCode; // Read-only, just for display
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

        private void SaveEdit()
        {
            if (SelectedProduct == null) return;

            SelectedProduct.Name = EditProductName.Trim();
            // ProductCode is auto-generated and read-only, so we don't update it
            SelectedProduct.Price = EditPrice.Value;
            SelectedProduct.LowStockLevel = EditLowStockLevel.Value;
            SelectedProduct.Description = EditDescription?.Trim() ?? string.Empty;
            SelectedProduct.CategoryName = EditSelectedCategory ?? SelectedProduct.CategoryName;

            // Refresh the Products collection to trigger UI update
            var temp = Products;
            Products = null;
            Products = temp;

            UpdateLowStockAlert();

            MessageBox.Show($"Product '{SelectedProduct.Name}' has been updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            IsEditModalVisible = false;
            ClearEditForm();
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

            // Use the edited name if available, otherwise use the product's current name
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

        private void AddStock()
        {
            if (SelectedProduct == null || !StockInQuantity.HasValue) return;

            SelectedProduct.StockQuanity += StockInQuantity.Value;

            // Refresh the Products collection to trigger UI update
            var temp = Products;
            Products = null;
            Products = temp;

            UpdateLowStockAlert();

            MessageBox.Show($"Added {StockInQuantity.Value} units to '{SelectedProduct.Name}'. New stock: {SelectedProduct.StockQuanity}",
                "Stock Added", MessageBoxButton.OK, MessageBoxImage.Information);

            CloseStockInModal();
        }

        private void OpenStockOutModal()
        {
            if (SelectedProduct == null) return;

            // Use the edited name if available, otherwise use the product's current name
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

        private void RemoveStock()
        {
            if (SelectedProduct == null || !StockOutQuantity.HasValue) return;

            // Check if there's enough stock to remove
            if (SelectedProduct.StockQuanity < StockOutQuantity.Value)
            {
                MessageBox.Show($"Cannot remove {StockOutQuantity.Value} units. Only {SelectedProduct.StockQuanity} units available.",
                    "Insufficient Stock", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedProduct.StockQuanity -= StockOutQuantity.Value;

            // Refresh the Products collection to trigger UI update
            var temp = Products;
            Products = null;
            Products = temp;

            UpdateLowStockAlert();

            MessageBox.Show($"Removed {StockOutQuantity.Value} units from '{SelectedProduct.Name}'. New stock: {SelectedProduct.StockQuanity}",
                "Stock Removed", MessageBoxButton.OK, MessageBoxImage.Information);

            CloseStockOutModal();
        }

        private void DeleteProduct(ProductViewModel product)
        {
            if (product == null) return;

            // Check if product has stock
            if (product.StockQuanity > 0)
            {
                MessageBox.Show($"Cannot delete '{product.Name}'. Product has {product.StockQuanity} units in stock.\nPlease remove all stock before deleting.",
                    "Cannot Delete Product", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Confirm deletion
            var result = MessageBox.Show($"Are you sure you want to delete '{product.Name}'?\nThis action cannot be undone.",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Remove from both collections
                allProducts.Remove(product);
                Products.Remove(product);

                MessageBox.Show($"Product '{product.Name}' has been deleted successfully.",
                    "Product Deleted", MessageBoxButton.OK, MessageBoxImage.Information);

                UpdateLowStockAlert();
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
