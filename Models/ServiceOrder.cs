using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gentech_services.Models
{
    public class ServiceOrder
    {
        [Key]
        public int ServiceOrderID { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public DateTime ScheduledAt { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // "Pending", "In Progress", "Completed", "Cancelled"

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? DeviceDescription { get; set; } // Device Name / Description

        public string? IssueNotes { get; set; } // Issue / Notes

        // ? NEW: Technician foreign key and navigation property
        public int? TechnicianID { get; set; }

        [ForeignKey("TechnicianID")]
        public virtual User? Technician { get; set; }

        // Navigation properties
        public virtual ICollection<ServiceOrderItem> ServiceOrderItems { get; set; } = new List<ServiceOrderItem>();

        // Backward compatibility properties (not mapped to database)
        [NotMapped]
        public string IssueDescription
        {
            get => IssueNotes ?? string.Empty;
            set => IssueNotes = value;
        }

        [NotMapped]
        public DateTime AppointmentDate
        {
            get => ScheduledAt;
            set => ScheduledAt = value;
        }

        [NotMapped]
        public int SaleID { get; set; } // Legacy property, not in new schema

        [NotMapped]
        public Service? Service { get; set; } // Legacy property - now uses ServiceOrderItems

        [NotMapped]
        public string? PaymentMethod { get; set; } // Legacy property

        [NotMapped]
        public Customer? Customer { get; set; } // Legacy property - now uses FullName, Email, Phone
    }
}