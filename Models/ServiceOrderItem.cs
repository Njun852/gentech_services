using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gentech_services.Models
{
    public class ServiceOrderItem
    {
        [Key]
        public int ServiceOrderItemID { get; set; }

        [Required]
        public int ServiceOrderID { get; set; }

        [Required]
        public int ServiceID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        // Navigation properties
        [ForeignKey("ServiceOrderID")]
        public virtual ServiceOrder? ServiceOrder { get; set; }

        [ForeignKey("ServiceID")]
        public virtual Service? Service { get; set; }
    }
}
