using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gentech_services.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Pin { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string Role { get; set; } = string.Empty;

        [Required]
        [MaxLength(25)]
        public string Username { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Backward compatibility properties (not mapped to database, computed from existing fields)
        [NotMapped]
        public string Name
        {
            get => FullName;
            set => FullName = value;
        }

        [NotMapped]
        public string Email { get; set; } = string.Empty; // Optional, not in schema but used by existing code

        [NotMapped]
        public string PasswordHash
        {
            get => Pin;
            set => Pin = value;
        }

        [NotMapped]
        public string PIN
        {
            get => Pin;
            set => Pin = value;
        }
    }
}
