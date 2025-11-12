using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gentech_services.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        //public ICollection<Product> Products { get; set; }
        //public ICollection<Service> Services { get; set; }
    }
}
