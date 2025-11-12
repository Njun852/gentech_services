using gentech_services.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductServicesManagementSystem.Models
{
    public class InventoryLog
    {
        public int LogID { get; set; }

        public int ProductID { get; set; }

        public int ChangeAmount { get; set; }

        public int UpdatedQuantity { get; set; }

        public string Reason { get; set; }

        public int RecordedByID { get; set; }

        public DateTime Timestamp { get; set; }

        // Navigation
        public Product Product { get; set; }
        public User RecordedBy { get; set; }
    }
}
