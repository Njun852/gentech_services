using gentech_services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductServicesManagementSystem.Models
{
    public class Sale
    {

        public int SaleID { get; set; }
        public int CustomerID { get; set; }
        public int StaffID { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public Customer Customer { get; set; }
        public User Staff { get; set; }
        //public ICollection<SaleItem> SaleItems { get; set; }
    }
}
