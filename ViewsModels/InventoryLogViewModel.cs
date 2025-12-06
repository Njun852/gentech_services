using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using gentech_services.MVVM;

namespace gentech_services.ViewsModels
{
    internal class InventoryLogViewModel : ViewModelBase
    {
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
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            allInventoryLogs = new ObservableCollection<InventoryLogEntry>
            {
                new InventoryLogEntry
                {
                    LogID = "LOG-001",
                    ProductID = "P001",
                    ProductName = "LENOVO LEGION 3",
                    AmountChange = "+16",
                    PreviousQty = 300,
                    UpdatedQty = 316,
                    Reason = "Sold",
                    RecordedBy = "Keith",
                    Timestamp = new DateTime(2025, 11, 11, 20, 0, 0),
                    ChangeColor = "#79F98C" // Green for positive
                },
                new InventoryLogEntry
                {
                    LogID = "LOG-002",
                    ProductID = "P001",
                    ProductName = "LENOVO LEGION 3",
                    AmountChange = "-5",
                    PreviousQty = 300,
                    UpdatedQty = 316,
                    Reason = "Broken",
                    RecordedBy = "Keith",
                    Timestamp = new DateTime(2025, 11, 11, 20, 0, 0),
                    ChangeColor = "#FF6586" // Red for negative
                },
                new InventoryLogEntry
                {
                    LogID = "LOG-003",
                    ProductID = "P002",
                    ProductName = "ASUS ROG STRIX",
                    AmountChange = "-5",
                    PreviousQty = 300,
                    UpdatedQty = 295,
                    Reason = "Allwel",
                    RecordedBy = "John",
                    Timestamp = new DateTime(2025, 11, 12, 10, 0, 0),
                    ChangeColor = "#FF6586"
                },
                new InventoryLogEntry
                {
                    LogID = "LOG-004",
                    ProductID = "P001",
                    ProductName = "LENOVO LEGION 3",
                    AmountChange = "+16",
                    PreviousQty = 300,
                    UpdatedQty = 295,
                    Reason = "GI napanaw",
                    RecordedBy = "Keith",
                    Timestamp = new DateTime(2025, 11, 13, 20, 2, 22),
                    ChangeColor = "#79F98C"
                },
                new InventoryLogEntry
                {
                    LogID = "LOG-005",
                    ProductID = "P003",
                    ProductName = "HP PAVILION",
                    AmountChange = "-5",
                    PreviousQty = 300,
                    UpdatedQty = 295,
                    Reason = "Reject",
                    RecordedBy = "Sarah",
                    Timestamp = new DateTime(2025, 11, 14, 14, 30, 0),
                    ChangeColor = "#FF6586"
                },
                new InventoryLogEntry
                {
                    LogID = "LOG-006",
                    ProductID = "P001",
                    ProductName = "LENOVO LEGION 3",
                    AmountChange = "-5",
                    PreviousQty = 300,
                    UpdatedQty = 295,
                    Reason = "Defective",
                    RecordedBy = "Keith",
                    Timestamp = new DateTime(2025, 11, 15, 9, 0, 0),
                    ChangeColor = "#FF6586"
                },
                new InventoryLogEntry
                {
                    LogID = "LOG-007",
                    ProductID = "P002",
                    ProductName = "ASUS ROG STRIX",
                    AmountChange = "+16",
                    PreviousQty = 300,
                    UpdatedQty = 316,
                    Reason = "Sold",
                    RecordedBy = "John",
                    Timestamp = new DateTime(2025, 11, 16, 16, 0, 0),
                    ChangeColor = "#79F98C"
                }
            };

            InventoryLogs = new ObservableCollection<InventoryLogEntry>(allInventoryLogs);
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
