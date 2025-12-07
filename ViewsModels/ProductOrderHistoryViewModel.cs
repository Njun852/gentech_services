using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using gentech_services.MVVM;
using gentech_services.Services;
using gentech_services.Models;

namespace gentech_services.ViewsModels
{
    internal class ProductOrderHistoryViewModel : ViewModelBase
    {
        private readonly ProductOrderService _productOrderService;

        private ObservableCollection<ProductOrderHistoryItem> allOrders;
        private ObservableCollection<ProductOrderHistoryItem> filteredOrders;
        private string searchText;
        private string selectedPaymentStatus;
        private DateTime? selectedDate;
        private bool isOrderDetailsVisible;
        private ObservableCollection<OrderDetailItem> orderDetailsItems;
        private decimal orderDetailsTotal;
        private bool isReturnModalVisible;
        private ObservableCollection<ReturnItem> returnItems;
        private decimal totalRefund;
        private bool isVoidModalVisible;
        private ProductOrderHistoryItem selectedOrderForVoid;
        private ProductOrderHistoryItem currentSelectedOrder;

        public bool IsVoidModalVisible
        {
            get { return isVoidModalVisible; }
            set
            {
                isVoidModalVisible = value;
                OnPropertyChanged(nameof(IsVoidModalVisible));
            }
        }

        public ProductOrderHistoryItem SelectedOrderForVoid
        {
            get { return selectedOrderForVoid; }
            set
            {
                selectedOrderForVoid = value;
                OnPropertyChanged(nameof(SelectedOrderForVoid));
            }
        }

        public ObservableCollection<ProductOrderHistoryItem> FilteredOrders
        {
            get { return filteredOrders; }
            set
            {
                filteredOrders = value;
                OnPropertyChanged(nameof(FilteredOrders));
            }
        }

        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ApplyFilters();
            }
        }

        public string SelectedPaymentStatus
        {
            get { return selectedPaymentStatus; }
            set
            {
                selectedPaymentStatus = value;
                OnPropertyChanged(nameof(SelectedPaymentStatus));
                ApplyFilters();
            }
        }

        public DateTime? SelectedDate
        {
            get { return selectedDate; }
            set
            {
                selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                ApplyFilters();
            }
        }

        public bool IsOrderDetailsVisible
        {
            get { return isOrderDetailsVisible; }
            set
            {
                isOrderDetailsVisible = value;
                OnPropertyChanged(nameof(IsOrderDetailsVisible));
            }
        }

        public ObservableCollection<OrderDetailItem> OrderDetailsItems
        {
            get { return orderDetailsItems; }
            set
            {
                orderDetailsItems = value;
                OnPropertyChanged(nameof(OrderDetailsItems));
            }
        }

        public decimal OrderDetailsTotal
        {
            get { return orderDetailsTotal; }
            set
            {
                orderDetailsTotal = value;
                OnPropertyChanged(nameof(OrderDetailsTotal));
            }
        }

        public bool IsReturnModalVisible
        {
            get { return isReturnModalVisible; }
            set
            {
                isReturnModalVisible = value;
                OnPropertyChanged(nameof(IsReturnModalVisible));
            }
        }

        public ObservableCollection<ReturnItem> ReturnItems
        {
            get { return returnItems; }
            set
            {
                returnItems = value;
                OnPropertyChanged(nameof(ReturnItems));
            }
        }

        public decimal TotalRefund
        {
            get { return totalRefund; }
            set
            {
                totalRefund = value;
                OnPropertyChanged(nameof(TotalRefund));
            }
        }

        // Commands
        public RelayCommand ShowOrderDetailsCommand { get; private set; }
        public RelayCommand CloseOrderDetailsCommand { get; private set; }
        public RelayCommand ShowReturnModalCommand { get; private set; }
        public RelayCommand CloseReturnModalCommand { get; private set; }
        public RelayCommand IncreaseReturnQuantityCommand { get; private set; }
        public RelayCommand DecreaseReturnQuantityCommand { get; private set; }
        public RelayCommand ProcessReturnCommand { get; private set; }
        public RelayCommand ShowVoidModalCommand { get; private set; }
        public RelayCommand CloseVoidModalCommand { get; private set; }
        public RelayCommand VoidTransactionCommand { get; private set; }

        public ProductOrderHistoryViewModel(ProductOrderService productOrderService)
        {
            _productOrderService = productOrderService;

            ShowOrderDetailsCommand = new RelayCommand(obj => ShowOrderDetails(obj as ProductOrderHistoryItem));
            CloseOrderDetailsCommand = new RelayCommand(obj => CloseOrderDetails());
            ShowReturnModalCommand = new RelayCommand(obj => ShowReturnModal(obj as ProductOrderHistoryItem));
            CloseReturnModalCommand = new RelayCommand(obj => CloseReturnModal());
            IncreaseReturnQuantityCommand = new RelayCommand(obj => IncreaseReturnQuantity(obj as ReturnItem));
            DecreaseReturnQuantityCommand = new RelayCommand(obj => DecreaseReturnQuantity(obj as ReturnItem));
            ProcessReturnCommand = new RelayCommand(obj => ProcessReturn());
            ShowVoidModalCommand = new RelayCommand(obj => ShowVoidModal(obj as ProductOrderHistoryItem));
            CloseVoidModalCommand = new RelayCommand(obj => CloseVoidModal());
            VoidTransactionCommand = new RelayCommand(obj => VoidTransaction());

            OrderDetailsItems = new ObservableCollection<OrderDetailItem>();
            ReturnItems = new ObservableCollection<ReturnItem>();
            _ = LoadOrdersFromDatabase();
        }

        private async System.Threading.Tasks.Task LoadOrdersFromDatabase()
        {
            try
            {
                var orders = await _productOrderService.GetAllProductOrdersAsync();

                allOrders = new ObservableCollection<ProductOrderHistoryItem>(
                    orders.Select(o => new ProductOrderHistoryItem
                    {
                        OrderNumber = $"PO-{o.ProductOrderID:0000}",
                        CustomerName = o.FullName,
                        OrderDate = o.CreatedAt,
                        TotalItems = o.ProductOrderItems?.Sum(oi => oi.Quantity) ?? 0,
                        TotalCost = o.TotalAmount,
                        PaymentStatus = o.Status,
                        ProductOrderID = o.ProductOrderID,
                        Items = new ObservableCollection<OrderDetailItem>(
                            o.ProductOrderItems?.Select(oi => new OrderDetailItem
                            {
                                ProductId = oi.Product?.SKU ?? $"P{oi.ProductID}",
                                ProductIDInt = oi.ProductID,
                                Name = oi.Product?.Name ?? "Unknown Product",
                                Category = oi.Product?.Category?.Name ?? "Uncategorized",
                                Quantity = oi.Quantity,
                                ReturnedQuantity = oi.ReturnedQuantity,
                                UnitPrice = oi.UnitPrice,
                                Subtotal = oi.TotalPrice
                            }) ?? Enumerable.Empty<OrderDetailItem>()
                        )
                    })
                );

                FilteredOrders = new ObservableCollection<ProductOrderHistoryItem>(allOrders);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                allOrders = new ObservableCollection<ProductOrderHistoryItem>();
                FilteredOrders = new ObservableCollection<ProductOrderHistoryItem>();
            }
        }

        private void LoadOrdersFallback()
        {
            allOrders = new ObservableCollection<ProductOrderHistoryItem>
            {
                new ProductOrderHistoryItem
                {
                    OrderNumber = "PO-001",
                    CustomerName = "Technician",
                    OrderDate = new DateTime(2025, 5, 25),
                    TotalItems = 3,
                    TotalCost = 33800m,
                    PaymentStatus = "Paid",
                    Items = new ObservableCollection<OrderDetailItem>
                    {
                        new OrderDetailItem { ProductId = "P2512001", Name = "AMD Ryzen 5 5600", Category = "Processor", Quantity = 2, ReturnedQuantity = 0, UnitPrice = 8950, Subtotal = 17900 },
                        new OrderDetailItem { ProductId = "P2512002", Name = "ASUS ROG STRIX B550-F", Category = "Motherboard", Quantity = 1, ReturnedQuantity = 0, UnitPrice = 9500, Subtotal = 9500 },
                        new OrderDetailItem { ProductId = "P2512003", Name = "Corsair Vengeance 16GB DDR4", Category = "RAM", Quantity = 2, ReturnedQuantity = 0, UnitPrice = 3200, Subtotal = 6400 }
                    }
                },
                new ProductOrderHistoryItem
                {
                    OrderNumber = "PO-002",
                    CustomerName = "Walk-in Customer",
                    OrderDate = new DateTime(2025, 5, 26),
                    TotalItems = 2,
                    TotalCost = 21800m,
                    PaymentStatus = "Paid",
                    Items = new ObservableCollection<OrderDetailItem>
                    {
                        new OrderDetailItem { ProductId = "P2512004", Name = "Samsung 970 EVO 1TB NVMe", Category = "Storage", Quantity = 2, ReturnedQuantity = 0, UnitPrice = 6900, Subtotal = 13800 },
                        new OrderDetailItem { ProductId = "P2512005", Name = "Cooler Master Hyper 212", Category = "Cooling", Quantity = 1, ReturnedQuantity = 0, UnitPrice = 2500, Subtotal = 2500 },
                        new OrderDetailItem { ProductId = "P2512006", Name = "Corsair RM750x PSU", Category = "Power Supply", Quantity = 1, ReturnedQuantity = 0, UnitPrice = 5500, Subtotal = 5500 }
                    }
                },
                new ProductOrderHistoryItem
                {
                    OrderNumber = "PO-003",
                    CustomerName = "John Doe",
                    OrderDate = new DateTime(2025, 6, 9),
                    TotalItems = 4,
                    TotalCost = 28400m,
                    PaymentStatus = "Partially Returned",
                    Items = new ObservableCollection<OrderDetailItem>
                    {
                        new OrderDetailItem { ProductId = "P2512007", Name = "Intel Core i7-12700K", Category = "Processor", Quantity = 1, ReturnedQuantity = 0, UnitPrice = 18500, Subtotal = 18500 },
                        new OrderDetailItem { ProductId = "P2512008", Name = "MSI Z690 Gaming Edge", Category = "Motherboard", Quantity = 1, ReturnedQuantity = 1, UnitPrice = 12500, Subtotal = 12500 },
                        new OrderDetailItem { ProductId = "P2512009", Name = "G.Skill Trident Z 32GB DDR5", Category = "RAM", Quantity = 1, ReturnedQuantity = 0, UnitPrice = 9800, Subtotal = 9800 }
                    }
                },
                new ProductOrderHistoryItem
                {
                    OrderNumber = "PO-004",
                    CustomerName = "Jane Smith",
                    OrderDate = new DateTime(2025, 6, 10),
                    TotalItems = 2,
                    TotalCost = 15200m,
                    PaymentStatus = "Paid",
                    Items = new ObservableCollection<OrderDetailItem>
                    {
                        new OrderDetailItem { ProductId = "P2512010", Name = "NVIDIA RTX 3060 Ti", Category = "Graphics Card", Quantity = 1, ReturnedQuantity = 0, UnitPrice = 22500, Subtotal = 22500 }
                    }
                },
                new ProductOrderHistoryItem
                {
                    OrderNumber = "PO-005",
                    CustomerName = "Robert Brown",
                    OrderDate = new DateTime(2025, 6, 11),
                    TotalItems = 3,
                    TotalCost = 19200m,
                    PaymentStatus = "Fully Returned",
                    Items = new ObservableCollection<OrderDetailItem>
                    {
                        new OrderDetailItem { ProductId = "P2512011", Name = "WD Black 2TB HDD", Category = "Storage", Quantity = 2, ReturnedQuantity = 2, UnitPrice = 3800, Subtotal = 7600 },
                        new OrderDetailItem { ProductId = "P2512012", Name = "Fractal Design Meshify C", Category = "Case", Quantity = 1, ReturnedQuantity = 1, UnitPrice = 4500, Subtotal = 4500 }
                    }
                },
                new ProductOrderHistoryItem
                {
                    OrderNumber = "PO-006",
                    CustomerName = "Emily Davis",
                    OrderDate = new DateTime(2025, 6, 12),
                    TotalItems = 1,
                    TotalCost = 8950m,
                    PaymentStatus = "Voided",
                    Items = new ObservableCollection<OrderDetailItem>
                    {
                        new OrderDetailItem { ProductId = "P2512001", Name = "AMD Ryzen 5 5600", Category = "Processor", Quantity = 1, ReturnedQuantity = 0, UnitPrice = 8950, Subtotal = 8950 }
                    }
                }
            };

            FilteredOrders = new ObservableCollection<ProductOrderHistoryItem>(allOrders);
        }

        private void ApplyFilters()
        {
            var filtered = allOrders.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string search = searchText.ToLower().Trim();
                filtered = filtered.Where(o =>
                    o.OrderNumber.ToLower().Contains(search) ||
                    o.CustomerName.ToLower().Contains(search));
            }

            // Apply payment status filter
            if (!string.IsNullOrWhiteSpace(selectedPaymentStatus) && selectedPaymentStatus != "All Status")
            {
                filtered = filtered.Where(o => o.PaymentStatus == selectedPaymentStatus);
            }

            // Apply date filter
            if (selectedDate.HasValue)
            {
                filtered = filtered.Where(o => o.OrderDate.Date == selectedDate.Value.Date);
            }

            FilteredOrders = new ObservableCollection<ProductOrderHistoryItem>(filtered);
        }

        private void ShowOrderDetails(ProductOrderHistoryItem order)
        {
            if (order == null) return;

            currentSelectedOrder = order;
            OrderDetailsItems = new ObservableCollection<OrderDetailItem>(order.Items);
            OrderDetailsTotal = OrderDetailsItems.Sum(item => item.Subtotal);
            IsOrderDetailsVisible = true;
        }

        private void CloseOrderDetails()
        {
            IsOrderDetailsVisible = false;
        }

        private void ShowReturnModal(ProductOrderHistoryItem order)
        {
            if (order == null) return;

            // Prevent returns for voided orders
            if (order.PaymentStatus == "Voided")
            {
                MessageBox.Show("Cannot process returns for voided transactions.", "Invalid Operation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            currentSelectedOrder = order;

            // Only show items with available quantity (not fully returned)
            ReturnItems = new ObservableCollection<ReturnItem>();
            foreach (var item in order.Items)
            {
                int availableQuantity = item.Quantity - item.ReturnedQuantity;
                if (availableQuantity > 0)
                {
                    ReturnItems.Add(new ReturnItem
                    {
                        ProductName = item.Name,
                        OriginalQuantity = item.Quantity,
                        ReturnedQuantity = item.ReturnedQuantity,
                        UnitPrice = item.UnitPrice,
                        ReturnQuantity = 0
                    });
                }
            }

            if (ReturnItems.Count == 0)
            {
                MessageBox.Show("All items in this order have been fully returned.", "No Items Available", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            CalculateTotalRefund();
            IsReturnModalVisible = true;
        }

        private void CloseReturnModal()
        {
            IsReturnModalVisible = false;
        }

        private void IncreaseReturnQuantity(ReturnItem item)
        {
            if (item != null && item.ReturnQuantity < item.AvailableQuantity)
            {
                item.ReturnQuantity++;
                CalculateTotalRefund();
            }
        }

        private void DecreaseReturnQuantity(ReturnItem item)
        {
            if (item != null && item.ReturnQuantity > 0)
            {
                item.ReturnQuantity--;
                CalculateTotalRefund();
            }
        }

        private void CalculateTotalRefund()
        {
            TotalRefund = ReturnItems.Sum(item => item.ReturnQuantity * item.UnitPrice);
        }

        private async void ProcessReturn()
        {
            var totalItems = ReturnItems.Sum(item => item.ReturnQuantity);
            if (totalItems == 0)
            {
                MessageBox.Show("Please select at least one item to return.", "No Items Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Process return for {totalItems} item(s)?\n\nTotal Refund: ₱{TotalRefund:N0}",
                "Confirm Return",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Prepare return items for service
                    var returnItemsList = ReturnItems
                        .Where(r => r.ReturnQuantity > 0)
                        .Select(r => {
                            var orderItem = currentSelectedOrder.Items.FirstOrDefault(i => i.Name == r.ProductName);
                            return (ProductID: orderItem?.ProductIDInt ?? 0, Quantity: r.ReturnQuantity);
                        })
                        .Where(r => r.ProductID > 0)
                        .ToList();

                    // Process return through service (will restore stock and log to inventory)
                    await _productOrderService.ProcessReturnAsync(
                        currentSelectedOrder.ProductOrderID,
                        returnItemsList
                    );

                    // Reload orders from database to get updated status
                    await LoadOrdersFromDatabase();

                    MessageBox.Show("Return processed successfully!\n\nStock has been restored and logged in inventory.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseReturnModal();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to process return: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateOrderPaymentStatus(ProductOrderHistoryItem order)
        {
            if (order == null || order.Items == null || order.Items.Count == 0) return;

            // Check if all items are fully returned
            bool allFullyReturned = order.Items.All(item => item.ReturnedQuantity >= item.Quantity);

            // Check if any items are partially returned
            bool anyReturned = order.Items.Any(item => item.ReturnedQuantity > 0);

            if (allFullyReturned)
            {
                order.PaymentStatus = "Fully Returned";
            }
            else if (anyReturned)
            {
                order.PaymentStatus = "Partially Returned";
            }
            else
            {
                order.PaymentStatus = "Paid";
            }
        }

        private void ShowVoidModal(ProductOrderHistoryItem order)
        {
            if (order == null) return;

            // Prevent voiding already voided orders
            if (order.PaymentStatus == "Voided")
            {
                MessageBox.Show("This transaction has already been voided.", "Invalid Operation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Prevent voiding orders with returns
            if (order.PaymentStatus == "Partially Returned" || order.PaymentStatus == "Fully Returned")
            {
                MessageBox.Show("Cannot void orders that have returns processed. Please use the return function instead.", "Invalid Operation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            currentSelectedOrder = order;
            OrderDetailsItems = new ObservableCollection<OrderDetailItem>(order.Items);
            OrderDetailsTotal = OrderDetailsItems.Sum(item => item.Subtotal);
            SelectedOrderForVoid = order;
            IsVoidModalVisible = true;
        }

        private void CloseVoidModal()
        {
            IsVoidModalVisible = false;
            SelectedOrderForVoid = null;
        }

        private async void VoidTransaction()
        {
            if (SelectedOrderForVoid == null) return;

            var result = MessageBox.Show(
                "Are you sure you want to void this transaction?\n\nThis will restore all stock quantities and cannot be undone.",
                "Confirm Void Transaction",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Void order through service (will restore stock and log to inventory)
                    await _productOrderService.VoidOrderAsync(SelectedOrderForVoid.ProductOrderID);

                    // Reload orders from database to get updated status
                    await LoadOrdersFromDatabase();

                    MessageBox.Show("Transaction voided successfully!\n\nStock has been restored and logged in inventory.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseVoidModal();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to void transaction: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    // Model
    public class ProductOrderHistoryItem : INotifyPropertyChanged
    {
        private string orderNumber;
        private string customerName;
        private DateTime orderDate;
        private int totalItems;
        private decimal totalCost;
        private string paymentStatus;
        private ObservableCollection<OrderDetailItem> items;

        public int ProductOrderID { get; set; }

        public string OrderNumber
        {
            get { return orderNumber; }
            set
            {
                orderNumber = value;
                OnPropertyChanged(nameof(OrderNumber));
            }
        }

        public string CustomerName
        {
            get { return customerName; }
            set
            {
                customerName = value;
                OnPropertyChanged(nameof(CustomerName));
            }
        }

        public DateTime OrderDate
        {
            get { return orderDate; }
            set
            {
                orderDate = value;
                OnPropertyChanged(nameof(OrderDate));
            }
        }

        public int TotalItems
        {
            get { return totalItems; }
            set
            {
                totalItems = value;
                OnPropertyChanged(nameof(TotalItems));
            }
        }

        public decimal TotalCost
        {
            get { return totalCost; }
            set
            {
                totalCost = value;
                OnPropertyChanged(nameof(TotalCost));
            }
        }

        public string PaymentStatus
        {
            get { return paymentStatus; }
            set
            {
                paymentStatus = value;
                OnPropertyChanged(nameof(PaymentStatus));
            }
        }

        public ObservableCollection<OrderDetailItem> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Order Detail Item Model
    public class OrderDetailItem : INotifyPropertyChanged
    {
        private string productId;
        private string name;
        private string category;
        private int quantity;
        private int returnedQuantity;
        private decimal unitPrice;
        private decimal subtotal;

        public int ProductIDInt { get; set; }

        public string ProductId
        {
            get { return productId; }
            set
            {
                productId = value;
                OnPropertyChanged(nameof(ProductId));
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

        public string Category
        {
            get { return category; }
            set
            {
                category = value;
                OnPropertyChanged(nameof(Category));
            }
        }

        public int Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(QuantityDisplay));
            }
        }

        public int ReturnedQuantity
        {
            get { return returnedQuantity; }
            set
            {
                returnedQuantity = value;
                OnPropertyChanged(nameof(ReturnedQuantity));
                OnPropertyChanged(nameof(HasReturns));
                OnPropertyChanged(nameof(QuantityDisplay));
            }
        }

        public bool HasReturns => returnedQuantity > 0;

        public string QuantityDisplay
        {
            get
            {
                if (returnedQuantity > 0)
                {
                    return $"{quantity} ({returnedQuantity} returned)";
                }
                return quantity.ToString();
            }
        }

        public decimal UnitPrice
        {
            get { return unitPrice; }
            set
            {
                unitPrice = value;
                OnPropertyChanged(nameof(UnitPrice));
            }
        }

        public decimal Subtotal
        {
            get { return subtotal; }
            set
            {
                subtotal = value;
                OnPropertyChanged(nameof(Subtotal));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Return Item Model
    public class ReturnItem : INotifyPropertyChanged
    {
        private string productName;
        private int originalQuantity;
        private int returnedQuantity;
        private decimal unitPrice;
        private int returnQuantity;

        public string ProductName
        {
            get { return productName; }
            set
            {
                productName = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }

        public int OriginalQuantity
        {
            get { return originalQuantity; }
            set
            {
                originalQuantity = value;
                OnPropertyChanged(nameof(OriginalQuantity));
                OnPropertyChanged(nameof(AvailableQuantity));
            }
        }

        public int ReturnedQuantity
        {
            get { return returnedQuantity; }
            set
            {
                returnedQuantity = value;
                OnPropertyChanged(nameof(ReturnedQuantity));
                OnPropertyChanged(nameof(AvailableQuantity));
            }
        }

        public int AvailableQuantity => originalQuantity - returnedQuantity;

        public decimal UnitPrice
        {
            get { return unitPrice; }
            set
            {
                unitPrice = value;
                OnPropertyChanged(nameof(UnitPrice));
            }
        }

        public int ReturnQuantity
        {
            get { return returnQuantity; }
            set
            {
                returnQuantity = value;
                OnPropertyChanged(nameof(ReturnQuantity));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
