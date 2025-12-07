using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gentech_services.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ProductCode { get; set; } // Auto-generated format: PYYMM XXX
        public int StockQuanity { get; set; }
        public int LowStockLevel { get; set; } // Threshold for low stock alerts
        public int CategoryID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public Category Category { get; set; }
       //public ICollection<SaleItem> SaleItems { get; set; }
       //public ICollection<InventoryLog> InventoryLogs { get; set; }
    }
}
