using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gentech_services.Models
{
    public class InventoryLog
    {
        [Key]
        public int InventoryLogID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        [MaxLength(20)]
        public string ChangeType { get; set; } = string.Empty; // "Stock In", "Stock Out", "Adjustment", "Sale", "Return"

        [Required]
        public int QuantityChanged { get; set; }

        [Required]
        public int PreviousQuantity { get; set; }

        [Required]
        public int NewQuantity { get; set; }

        public string? Reason { get; set; }

        [Required]
        public int CreatedBy { get; set; } // UserID

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("ProductID")]
        public virtual Product? Product { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User? User { get; set; }
    }
}
