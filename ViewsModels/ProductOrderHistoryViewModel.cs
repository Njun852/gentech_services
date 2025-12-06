using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using gentech_services.MVVM;

namespace gentech_services.ViewsModels
{
    internal class ProductOrderHistoryViewModel : ViewModelBase
    {
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

        public ProductOrderHistoryViewModel()
        {
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
            LoadOrders();
        }

        private void LoadOrders()
        {
            allOrders = new ObservableCollection<ProductOrderHistoryItem>
            {
                new ProductOrderHistoryItem
                {
                    OrderNumber = "PO-001",
                    CustomerName = "Technician",
                    OrderDate = new DateTime(2025, 5, 25),
                    TotalItems = 3,
                    TotalCost = 14.856m,
                    PaymentStatus = "Paid"
                },
                new ProductOrderHistoryItem
                {
                    OrderNumber = "PO-001",
                    CustomerName = "Technician",
                    OrderDate = new DateTime(2025, 5, 25),
                    TotalItems = 3,
                    TotalCost = 14.856m,
                    PaymentStatus = "Paid"
                },
                new ProductOrderHistoryItem
                {
                    OrderNumber = "PO-001",
                    CustomerName = "Technician",
                    OrderDate = new DateTime(2025, 6, 9),
                    TotalItems = 3,
                    TotalCost = 14.856m,
                    PaymentStatus = "Paid"
                },
                new ProductOrderHistoryItem
                {
                    OrderNumber = "PO-001",
                    CustomerName = "Technician",
                    OrderDate = new DateTime(2025, 6, 9),
                    TotalItems = 3,
                    TotalCost = 14.856m,
                    PaymentStatus = "Paid"
                },
                new ProductOrderHistoryItem
                {
                    OrderNumber = "PO-001",
                    CustomerName = "Technician",
                    OrderDate = new DateTime(2025, 5, 25),
                    TotalItems = 3,
                    TotalCost = 14.856m,
                    PaymentStatus = "Paid"
                },
                new ProductOrderHistoryItem
                {
                    OrderNumber = "PO-001",
                    CustomerName = "Technician",
                    OrderDate = new DateTime(2025, 5, 25),
                    TotalItems = 3,
                    TotalCost = 14.856m,
                    PaymentStatus = "Paid"
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

            // Load sample order details
            OrderDetailsItems = new ObservableCollection<OrderDetailItem>
            {
                new OrderDetailItem { ProductId = "P001", Name = "AMD Ryzen 5 5600", Category = "Processor", Quantity = 2, ReturnedQuantity = 0, UnitPrice = 8950, Subtotal = 17900 },
                new OrderDetailItem { ProductId = "P002", Name = "ASUS ROG STRIX B550-F", Category = "Motherboard", Quantity = 1, ReturnedQuantity = 1, UnitPrice = 9500, Subtotal = 9500 },
                new OrderDetailItem { ProductId = "P003", Name = "Corsair Vengeance 16GB DDR4", Category = "RAM", Quantity = 2, ReturnedQuantity = 0, UnitPrice = 3200, Subtotal = 6400 }
            };

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

            // Load sample return items - only show items with available quantity (not fully returned)
            ReturnItems = new ObservableCollection<ReturnItem>
            {
                new ReturnItem { ProductName = "AMD Ryzen 5 5600", OriginalQuantity = 2, ReturnedQuantity = 0, UnitPrice = 8950, ReturnQuantity = 0 },
                // ASUS ROG STRIX B550-F is excluded (fully returned: OriginalQuantity=1, ReturnedQuantity=1)
                new ReturnItem { ProductName = "Corsair Vengeance 16GB DDR4", OriginalQuantity = 2, ReturnedQuantity = 0, UnitPrice = 3200, ReturnQuantity = 0 }
            };

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

        private void ProcessReturn()
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
                MessageBox.Show("Return processed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseReturnModal();
            }
        }

        private void ShowVoidModal(ProductOrderHistoryItem order)
        {
            if (order == null) return;

            // Load sample order details
            OrderDetailsItems = new ObservableCollection<OrderDetailItem>
            {
                new OrderDetailItem { ProductId = "P001", Name = "AMD Ryzen 5 5600", Category = "Processor", Quantity = 2, ReturnedQuantity = 0, UnitPrice = 8950, Subtotal = 17900 },
                new OrderDetailItem { ProductId = "P002", Name = "ASUS ROG STRIX B550-F", Category = "Motherboard", Quantity = 1, ReturnedQuantity = 1, UnitPrice = 9500, Subtotal = 9500 },
                new OrderDetailItem { ProductId = "P003", Name = "Corsair Vengeance 16GB DDR4", Category = "RAM", Quantity = 2, ReturnedQuantity = 0, UnitPrice = 3200, Subtotal = 6400 }
            };

            OrderDetailsTotal = OrderDetailsItems.Sum(item => item.Subtotal);
            SelectedOrderForVoid = order;
            IsVoidModalVisible = true;
        }

        private void CloseVoidModal()
        {
            IsVoidModalVisible = false;
            SelectedOrderForVoid = null;
        }

        private void VoidTransaction()
        {
            if (SelectedOrderForVoid == null) return;

            var result = MessageBox.Show(
                "Are you sure you want to void this transaction? This action cannot be undone.",
                "Confirm Void Transaction",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Transaction voided successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseVoidModal();
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
            }
        }

        public bool HasReturns => returnedQuantity > 0;

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
