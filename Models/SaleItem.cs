using ProductServicesManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gentech_services.Models
{
    public class SaleItem
    {
        public int SaleItemID { get; set; }

        public int SaleID { get; set; }

        public int? ProductID { get; set; }

        public int? ServiceID { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }


        public decimal TotalPrice { get; set; }

        public Sale Sale { get; set; }
        public Product Product { get; set; }
        public Service Service { get; set; }
    }
}
