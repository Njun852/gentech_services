using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using gentech_services.Models;
using gentech_services.Services;
using gentech_services.Data;
using gentech_services.Repositories;
using Microsoft.EntityFrameworkCore;
using ProductServicesManagementSystem.Models;

namespace gentech_services.Views.Pages
{
    /// <summary>
    /// Interaction logic for ServiceManagementPage.xaml
    /// </summary>
    public partial class ServiceManagementPage : UserControl
    {
        public ObservableCollection<Service> Services { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
        private ObservableCollection<CategoryViewModel> categoryViewModels;
        private ObservableCollection<Service> filteredServices;
        private ServiceService _serviceService;
        private CategoryService _categoryService;
        private GentechDbContext _dbContext;

        public ServiceManagementPage()
        {
            InitializeComponent();
            InitializeDatabase();
            DataContext = this;

            // Wire up modal event
            AddServiceModalControl.OnServiceSaved += AddServiceModalControl_OnServiceSaved;

            // Wire up edit modal event
            EditServiceModalControl.OnServiceUpdated += EditServiceModalControl_OnServiceUpdated;

            // Wire up action menu events
            ServiceActionMenuControl.OnEdit += ServiceActionMenuControl_OnEdit;
            ServiceActionMenuControl.OnDelete += ServiceActionMenuControl_OnDelete;
        }

        private async void InitializeDatabase()
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<GentechDbContext>();
                optionsBuilder.UseSqlite("Data Source=gentech.db");

                _dbContext = new GentechDbContext(optionsBuilder.Options);
                await _dbContext.Database.EnsureCreatedAsync();

                var serviceRepository = new ServiceRepository(_dbContext);
                var categoryRepository = new CategoryRepository(_dbContext);
                _serviceService = new ServiceService(serviceRepository, categoryRepository);
                _categoryService = new CategoryService(categoryRepository);

                await LoadDataFromDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadDataFromDatabase()
        {
            try
            {
                // Load categories
                var categories = await _categoryService.GetServiceCategoriesAsync();
                Categories = new ObservableCollection<Category>(categories);

                // Load services
                var services = await _serviceService.GetAllServicesAsync();
                Services = new ObservableCollection<Service>(services);

                ServicesItemsControl.ItemsSource = Services;
                RefreshCategoryList();

                // Re-initialize modal with updated categories
                if (AddServiceModalControl != null)
                {
                    AddServiceModalControl.Initialize(Categories);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class CategoryViewModel
        {
            public int CategoryID { get; set; }
            public string Name { get; set; }
            public int ServicesCount { get; set; }
            public Category OriginalCategory { get; set; }
        }


        private void RefreshCategoryList()
        {
            // Create category view models with service counts
            categoryViewModels = new ObservableCollection<CategoryViewModel>(
                Categories.Select(c => new CategoryViewModel
                {
                    CategoryID = c.CategoryID,
                    Name = c.Name,
                    ServicesCount = Services.Count(s => s.CategoryID == c.CategoryID),
                    OriginalCategory = c
                })
            );

            CategoriesItemsControl.ItemsSource = categoryViewModels;
        }

        private void RenameCategory_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var categoryVM = button?.DataContext as CategoryViewModel;
            if (categoryVM == null) return;

            // Create input dialog
            var inputDialog = new Window
            {
                Title = "Rename Category",
                Width = 400,
                Height = 220,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.White
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
                Text = categoryVM.Name,
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
                Background = Brushes.White,
                BorderBrush = Brushes.Gray,
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
                Background = (Brush)new BrushConverter().ConvertFrom("#0000FF"),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            saveButton.Click += async (s, args) =>
            {
                var newName = textBox.Text.Trim();
                if (!string.IsNullOrEmpty(newName))
                {
                    try
                    {
                        // Update category in database
                        await _categoryService.UpdateCategoryAsync(
                            categoryId: categoryVM.CategoryID,
                            name: newName,
                            type: "Service"
                        );

                        // Reload data from database
                        await LoadDataFromDatabase();

                        inputDialog.Close();

                        MessageBox.Show($"Category renamed to '{newName}' successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to rename category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Category name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private async void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var categoryVM = button?.DataContext as CategoryViewModel;
            if (categoryVM == null) return;

            // Check if category has services
            if (categoryVM.ServicesCount > 0)
            {
                MessageBox.Show(
                    $"Cannot delete '{categoryVM.Name}' because it has {categoryVM.ServicesCount} service(s) assigned to it.\n\nPlease reassign or delete those services first.",
                    "Cannot Delete Category",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete the category '{categoryVM.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Delete from database
                    await _categoryService.DeleteCategoryAsync(categoryVM.CategoryID);

                    // Reload data from database
                    await LoadDataFromDatabase();

                    MessageBox.Show($"Category '{categoryVM.Name}' has been deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            var categoryName = NewCategoryTextBox.Text.Trim();

            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Please enter a category name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Create category in database
                await _categoryService.CreateCategoryAsync(categoryName, "Service");

                // Reload data from database
                await LoadDataFromDatabase();

                // Clear the textbox
                NewCategoryTextBox.Text = string.Empty;

                MessageBox.Show($"Category '{categoryName}' has been added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewService_Click(object sender, RoutedEventArgs e)
        {
            // Initialize modal with categories
            AddServiceModalControl.Initialize(Categories);

            // Show modal
            ModalOverlay.Visibility = Visibility.Visible;
        }

        private void ModalOverlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Close modal when clicking on overlay (outside the modal)
            if (e.OriginalSource == ModalOverlay)
            {
                ModalOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private void EditModalOverlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Close edit modal when clicking on overlay (outside the modal)
            if (e.OriginalSource == EditModalOverlay)
            {
                EditModalOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private async void EditServiceModalControl_OnServiceUpdated(object sender, Service updatedService)
        {
            try
            {
                // Update in database
                await _serviceService.UpdateServiceAsync(
                    serviceId: updatedService.ServiceID,
                    name: updatedService.Name,
                    description: updatedService.Description ?? string.Empty,
                    price: updatedService.Price,
                    categoryId: updatedService.CategoryID,
                    isActive: updatedService.IsActive
                );

                // Reload data from database
                await LoadDataFromDatabase();

                // Close edit modal
                EditModalOverlay.Visibility = Visibility.Collapsed;

                MessageBox.Show($"Service '{updatedService.Name}' has been updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating service: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddServiceModalControl_OnServiceSaved(object sender, Service newService)
        {
            try
            {
                // Create in database
                await _serviceService.CreateServiceAsync(
                    name: newService.Name,
                    description: newService.Description ?? string.Empty,
                    price: newService.Price,
                    categoryId: newService.CategoryID
                );

                // Reload data from database
                await LoadDataFromDatabase();

                // Close modal
                ModalOverlay.Visibility = Visibility.Collapsed;

                MessageBox.Show($"Service '{newService.Name}' has been added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding service: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ServiceActionButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var service = button?.DataContext as Service;
            if (service == null) return;

            // Set the service in the action menu
            ServiceActionMenuControl.SetService(service);

            // Position and show the popup
            ServiceActionPopup.PlacementTarget = button;
            ServiceActionPopup.IsOpen = true;
        }

        private void ServiceActionMenuControl_OnEdit(object sender, Service service)
        {
            ServiceActionPopup.IsOpen = false;

            // Initialize edit modal with categories and service
            EditServiceModalControl.Initialize(Categories, service);

            // Show edit modal
            EditModalOverlay.Visibility = Visibility.Visible;
        }

        private async void ServiceActionMenuControl_OnDelete(object sender, Service service)
        {
            ServiceActionPopup.IsOpen = false;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the service '{service.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Delete from database
                    await _serviceService.DeleteServiceAsync(service.ServiceID);

                    // Reload data from database
                    await LoadDataFromDatabase();

                    MessageBox.Show($"Service '{service.Name}' has been deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting service: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Show all services if search is empty
                ServicesItemsControl.ItemsSource = Services;
            }
            else
            {
                // Filter services based on search text
                filteredServices = new ObservableCollection<Service>(
                    Services.Where(s =>
                        s.Name.ToLower().Contains(searchText) ||
                        s.Description.ToLower().Contains(searchText) ||
                        s.Category.Name.ToLower().Contains(searchText) ||
                        s.Price.ToString().Contains(searchText)
                    )
                );

                ServicesItemsControl.ItemsSource = filteredServices;
            }
        }
    }
}
