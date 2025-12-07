using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using gentech_services.MVVM;
using gentech_services.Services;
using gentech_services.Repositories;
using gentech_services.Data;
using Microsoft.EntityFrameworkCore;

namespace gentech_services.ViewsModels
{
    internal class InventoryLogViewModel : ViewModelBase
    {
        private readonly InventoryLogService _inventoryLogService;
        private ObservableCollection<InventoryLogEntry> allInventoryLogs;
        private ObservableCollection<InventoryLogEntry> inventoryLogs;
        private string recordedByFilter;
        private string productFilter;
        private DateTime? startDate;
        private DateTime? endDate;

        public ObservableCollection<InventoryLogEntry> InventoryLogs
        {
            get { return inventoryLogs; }
            set
            {
                inventoryLogs = value;
                OnPropertyChanged(nameof(InventoryLogs));
                OnPropertyChanged(nameof(TotalLogEntries));
            }
        }

        public string RecordedByFilter
        {
            get { return recordedByFilter; }
            set
            {
                recordedByFilter = value;
                OnPropertyChanged(nameof(RecordedByFilter));
                ApplyFilters();
            }
        }

        public string ProductFilter
        {
            get { return productFilter; }
            set
            {
                productFilter = value;
                OnPropertyChanged(nameof(ProductFilter));
                ApplyFilters();
            }
        }

        public DateTime? StartDate
        {
            get { return startDate; }
            set
            {
                startDate = value;
                OnPropertyChanged(nameof(StartDate));
                ApplyFilters();
            }
        }

        public DateTime? EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
                OnPropertyChanged(nameof(EndDate));
                ApplyFilters();
            }
        }

        public int TotalLogEntries
        {
            get { return InventoryLogs?.Count ?? 0; }
        }

        public InventoryLogViewModel()
        {
            // Initialize service
            var optionsBuilder = new DbContextOptionsBuilder<GentechDbContext>();
            optionsBuilder.UseSqlite("Data Source=gentech.db");
            var context = new GentechDbContext(optionsBuilder.Options);
            var inventoryLogRepository = new InventoryLogRepository(context);
            _inventoryLogService = new InventoryLogService(inventoryLogRepository);

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var dbLogs = await _inventoryLogService.GetAllLogsAsync();

                allInventoryLogs = new ObservableCollection<InventoryLogEntry>(
                    dbLogs.Select(log => new InventoryLogEntry
                    {
                        LogID = $"L{log.InventoryLogID:0000}",
                        ProductID = log.Product?.SKU ?? "N/A",
                        ProductName = log.Product?.Name ?? "Unknown Product",
                        AmountChange = log.ChangeType == "Stock In"
                            ? $"+{log.QuantityChanged}"
                            : $"-{log.QuantityChanged}",
                        PreviousQty = log.PreviousQuantity,
                        UpdatedQty = log.NewQuantity,
                        Reason = log.Reason ?? log.ChangeType,
                        RecordedBy = log.User?.Username ?? "System",
                        Timestamp = log.CreatedAt,
                        ChangeColor = log.ChangeType == "Stock In" ? "#79F98C" : "#FF6586"
                    })
                );

                InventoryLogs = new ObservableCollection<InventoryLogEntry>(allInventoryLogs);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load inventory logs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                allInventoryLogs = new ObservableCollection<InventoryLogEntry>();
                InventoryLogs = new ObservableCollection<InventoryLogEntry>();
            }
        }

        private void ApplyFilters()
        {
            var filtered = allInventoryLogs.AsEnumerable();

            // Filter by recorded by
            if (!string.IsNullOrWhiteSpace(RecordedByFilter))
            {
                filtered = filtered.Where(log =>
                    log.RecordedBy.Contains(RecordedByFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by product name or ID
            if (!string.IsNullOrWhiteSpace(ProductFilter))
            {
                filtered = filtered.Where(log =>
                    log.ProductName.Contains(ProductFilter, StringComparison.OrdinalIgnoreCase) ||
                    log.ProductID.Contains(ProductFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by date range
            if (StartDate.HasValue)
            {
                filtered = filtered.Where(log => log.Timestamp.Date >= StartDate.Value.Date);
            }

            if (EndDate.HasValue)
            {
                filtered = filtered.Where(log => log.Timestamp.Date <= EndDate.Value.Date);
            }

            InventoryLogs = new ObservableCollection<InventoryLogEntry>(filtered);
        }
    }

    public class InventoryLogEntry : INotifyPropertyChanged
    {
        private string logID;
        private string productID;
        private string productName;
        private string amountChange;
        private int previousQty;
        private int updatedQty;
        private string reason;
        private string recordedBy;
        private DateTime timestamp;
        private string changeColor;

        public string LogID
        {
            get { return logID; }
            set
            {
                logID = value;
                OnPropertyChanged(nameof(LogID));
            }
        }

        public string ProductID
        {
            get { return productID; }
            set
            {
                productID = value;
                OnPropertyChanged(nameof(ProductID));
            }
        }

        public string ProductName
        {
            get { return productName; }
            set
            {
                productName = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }

        public string AmountChange
        {
            get { return amountChange; }
            set
            {
                amountChange = value;
                OnPropertyChanged(nameof(AmountChange));
            }
        }

        public int PreviousQty
        {
            get { return previousQty; }
            set
            {
                previousQty = value;
                OnPropertyChanged(nameof(PreviousQty));
            }
        }

        public int UpdatedQty
        {
            get { return updatedQty; }
            set
            {
                updatedQty = value;
                OnPropertyChanged(nameof(UpdatedQty));
            }
        }

        public string Reason
        {
            get { return reason; }
            set
            {
                reason = value;
                OnPropertyChanged(nameof(Reason));
            }
        }

        public string RecordedBy
        {
            get { return recordedBy; }
            set
            {
                recordedBy = value;
                OnPropertyChanged(nameof(RecordedBy));
            }
        }

        public DateTime Timestamp
        {
            get { return timestamp; }
            set
            {
                timestamp = value;
                OnPropertyChanged(nameof(Timestamp));
            }
        }

        public string ChangeColor
        {
            get { return changeColor; }
            set
            {
                changeColor = value;
                OnPropertyChanged(nameof(ChangeColor));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
