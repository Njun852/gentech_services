using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using gentech_services.ViewsModels;
using gentech_services.Models;
using gentech_services.Services;
using gentech_services.Repositories;
using gentech_services.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace gentech_services.Views.Pages
{
    public partial class InventoryManagementPage : UserControl
    {
        private InventoryManagementViewModel ViewModel => (InventoryManagementViewModel)DataContext;
        private ObservableCollection<ProductCategoryViewModel> productCategories;
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;

        public InventoryManagementPage()
        {
            InitializeComponent();
            DataContext = new InventoryManagementViewModel();

            // Initialize services
            var optionsBuilder = new DbContextOptionsBuilder<GentechDbContext>();
            optionsBuilder.UseSqlite("Data Source=gentech.db");
            var context = new GentechDbContext(optionsBuilder.Options);
            var categoryRepository = new CategoryRepository(context);
            var productRepository = new ProductRepository(context);
            var inventoryLogRepository = new InventoryLogRepository(context);
            var inventoryLogService = new InventoryLogService(inventoryLogRepository);

            _categoryService = new CategoryService(categoryRepository);
            _productService = new ProductService(productRepository, inventoryLogService);

            LoadProductCategories();

            // Subscribe to product collection changes to refresh category counts
            if (ViewModel != null && ViewModel.Products != null)
            {
                ViewModel.Products.CollectionChanged += (s, e) => LoadProductCategories();
            }
        }

        private void ProductActionButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.DataContext as ProductViewModel;

            if (product == null) return;

            // Store the selected product in a way that the popup menu can access it
            ProductActionPopup.Tag = product;
            ProductActionPopup.PlacementTarget = button;
            ProductActionPopup.IsOpen = true;
        }

        private void ViewProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductActionPopup.IsOpen = false;
            var product = ProductActionPopup.Tag as ProductViewModel;
            if (product != null)
            {
                ViewModel.ViewProductCommand.Execute(product);
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductActionPopup.IsOpen = false;
            var product = ProductActionPopup.Tag as ProductViewModel;
            if (product != null)
            {
                ViewModel.EditProductCommand.Execute(product);
            }
        }

        private void EditStockDetails_Click(object sender, RoutedEventArgs e)
        {
            ProductActionPopup.IsOpen = false;
            var product = ProductActionPopup.Tag as ProductViewModel;
            if (product != null)
            {
                ViewModel.EditProductCommand.Execute(product);
            }
        }

        private void ModalOverlay_Click(object sender, MouseButtonEventArgs e)
        {
            // Close the modal when clicking on the overlay
            ViewModel.CancelAddCommand.Execute(null);
        }

        private void EditModalOverlay_Click(object sender, MouseButtonEventArgs e)
        {
            // Close the edit modal when clicking on the overlay
            ViewModel.CloseEditModalCommand?.Execute(null);
        }

        private void EditModalContent_Click(object sender, MouseButtonEventArgs e)
        {
            // Prevent click from bubbling to overlay
            e.Handled = true;
        }

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductActionPopup.IsOpen = false;
            var product = ProductActionPopup.Tag as ProductViewModel;
            if (product != null)
            {
                ViewModel.DeleteProductCommand.Execute(product);
                // Refresh category counts after deletion
                await Task.Delay(500); // Small delay to ensure delete completes
                LoadProductCategories();
            }
        }

        // Product Category Management
        private async void LoadProductCategories()
        {
            try
            {
                var categories = await _categoryService.GetProductCategoriesAsync();
                var allProducts = await _productService.GetActiveProductsAsync();

                productCategories = new ObservableCollection<ProductCategoryViewModel>(
                    categories.Select(c => new ProductCategoryViewModel
                    {
                        CategoryID = c.CategoryID,
                        Name = c.Name,
                        ProductsCount = allProducts.Count(p => p.CategoryID == c.CategoryID)
                    })
                );

                CategoriesItemsControl.ItemsSource = productCategories;

                // Initialize ComboBoxes in Add/Edit modals
                RefreshCategoryComboBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddProductCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string categoryName = NewProductCategoryTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    MessageBox.Show("Please enter a category name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create category in database
                var category = await _categoryService.CreateCategoryAsync(categoryName, "Product");

                var newCategory = new ProductCategoryViewModel
                {
                    CategoryID = category.CategoryID,
                    Name = category.Name,
                    ProductsCount = 0
                };

                productCategories.Add(newCategory);
                NewProductCategoryTextBox.Text = string.Empty;

                // Update ComboBoxes in Add/Edit modals
                RefreshCategoryComboBoxes();

                MessageBox.Show($"Category '{categoryName}' has been added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RenameProductCategory_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag == null) return;

            int categoryId = (int)button.Tag;
            var category = productCategories.FirstOrDefault(c => c.CategoryID == categoryId);

            if (category == null) return;

            // Create input dialog matching Service Management style
            var inputDialog = new Window
            {
                Title = "Rename Category",
                Width = 400,
                Height = 220,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Background = System.Windows.Media.Brushes.White
            };

            var grid = new Grid { Margin = new Thickness(20) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var label = new TextBlock
            {
                Text = "Category Name:",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 8)
            };

            var textBox = new TextBox
            {
                Text = category.Name,
                Height = 35,
                FontSize = 13,
                Padding = new Thickness(8),
                Margin = new Thickness(0, 0, 0, 20),
                VerticalContentAlignment = VerticalAlignment.Center
            };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 100,
                Height = 35,
                Margin = new Thickness(0, 0, 10, 0),
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = System.Windows.Media.Brushes.Gray,
                BorderThickness = new Thickness(1),
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            cancelButton.Click += (s, args) => inputDialog.Close();

            var saveButton = new Button
            {
                Content = "Save",
                Width = 100,
                Height = 35,
                Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom("#0000FF"),
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            saveButton.Click += async (s, args) =>
            {
                try
                {
                    string newName = textBox.Text.Trim();
                    if (string.IsNullOrWhiteSpace(newName))
                    {
                        MessageBox.Show("Category name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string oldName = category.Name;

                    // Update category in database
                    await _categoryService.UpdateCategoryAsync(categoryId, newName, "Product");

                    category.Name = newName;

                    // Update all products that reference this category
                    foreach (var product in ViewModel.Products.Where(p => p.CategoryName == oldName))
                    {
                        product.CategoryName = newName;
                    }

                    // Refresh products table to show updated category names
                    var temp = ViewModel.Products;
                    ViewModel.Products = null;
                    ViewModel.Products = temp;

                    // Update ComboBoxes in Add/Edit modals
                    RefreshCategoryComboBoxes();

                    inputDialog.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to update category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            buttonPanel.Children.Add(cancelButton);
            buttonPanel.Children.Add(saveButton);

            Grid.SetRow(label, 0);
            Grid.SetRow(textBox, 1);
            Grid.SetRow(buttonPanel, 2);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
            grid.Children.Add(buttonPanel);

            inputDialog.Content = grid;
            textBox.Focus();
            textBox.SelectAll();
            inputDialog.ShowDialog();
        }

        private async void DeleteProductCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button?.Tag == null) return;

                int categoryId = (int)button.Tag;
                var category = productCategories.FirstOrDefault(c => c.CategoryID == categoryId);

                if (category == null) return;

                if (category.ProductsCount > 0)
                {
                    MessageBox.Show($"Cannot delete category '{category.Name}' because it has {category.ProductsCount} product(s) assigned to it.",
                        "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Are you sure you want to delete the category '{category.Name}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Delete from database
                    await _categoryService.DeleteCategoryAsync(categoryId);

                    productCategories.Remove(category);

                    // Update ComboBoxes in Add/Edit modals
                    RefreshCategoryComboBoxes();

                    MessageBox.Show($"Category '{category.Name}' has been deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshCategoryComboBoxes()
        {
            // Get all category names
            var categoryNames = productCategories.Select(c => c.Name).ToList();

            // Update Add Product Category ComboBox
            if (AddProductCategoryComboBox != null)
            {
                string selectedCategory = AddProductCategoryComboBox.SelectedValue?.ToString();
                AddProductCategoryComboBox.Items.Clear();
                foreach (var categoryName in categoryNames)
                {
                    var item = new ComboBoxItem { Content = categoryName };
                    AddProductCategoryComboBox.Items.Add(item);
                }
                // Restore selection if it still exists
                if (!string.IsNullOrEmpty(selectedCategory) && categoryNames.Contains(selectedCategory))
                {
                    AddProductCategoryComboBox.SelectedValue = selectedCategory;
                }
            }

            // Update Edit Product Category ComboBox
            if (EditProductCategoryComboBox != null)
            {
                string selectedCategory = EditProductCategoryComboBox.SelectedValue?.ToString();
                EditProductCategoryComboBox.Items.Clear();
                foreach (var categoryName in categoryNames)
                {
                    var item = new ComboBoxItem { Content = categoryName };
                    EditProductCategoryComboBox.Items.Add(item);
                }
                // Restore selection if it still exists
                if (!string.IsNullOrEmpty(selectedCategory) && categoryNames.Contains(selectedCategory))
                {
                    EditProductCategoryComboBox.SelectedValue = selectedCategory;
                }
            }
        }
    }

    // Product Category ViewModel
    public class ProductCategoryViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private int categoryID;
        private string name;
        private int productsCount;

        public int CategoryID
        {
            get { return categoryID; }
            set
            {
                categoryID = value;
                OnPropertyChanged(nameof(CategoryID));
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

        public int ProductsCount
        {
            get { return productsCount; }
            set
            {
                productsCount = value;
                OnPropertyChanged(nameof(ProductsCount));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
