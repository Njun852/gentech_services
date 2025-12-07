using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gentech_services.Models
{
    public class ProductOrderItem
    {
        [Key]
        public int ProductOrderItemID { get; set; }

        [Required]
        public int ProductOrderID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        // Navigation properties
        [ForeignKey("ProductOrderID")]
        public virtual ProductOrder? ProductOrder { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product? Product { get; set; }
    }
}
