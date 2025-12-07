using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gentech_services.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Type { get; set; } = string.Empty; // "Product" or "Service"

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
