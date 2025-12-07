using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gentech_services.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty; // Product Code format: PYYMM XXX

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public int LowStockLevel { get; set; } // Threshold for low stock alerts

        [Required]
        public int CategoryID { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("CategoryID")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<ProductOrderItem> ProductOrderItems { get; set; } = new List<ProductOrderItem>();
        public virtual ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();

        // Backward compatibility properties (not mapped to database)
        [NotMapped]
        public string ProductCode
        {
            get => SKU;
            set => SKU = value;
        }

        [NotMapped]
        public int StockQuanity
        {
            get => StockQuantity;
            set => StockQuantity = value;
        }
    }
}
