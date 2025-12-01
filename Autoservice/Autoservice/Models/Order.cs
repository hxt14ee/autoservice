using System;
using System.Collections.Generic;

namespace Autoservice.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CarID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime? DateCompleted { get; set; }
        public string Status { get; set; } 
        public decimal TotalAmount { get; set; }

        public virtual Car Car { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<OrderPart> OrderParts { get; set; }
        public virtual ICollection<OrderService> OrderServices { get; set; }
    }
}
