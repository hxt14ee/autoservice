using System;

namespace Autoservice.Models
{
    public class OrderService
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public int ServiceID { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }

        public virtual Order Order { get; set; }
        public virtual Service Service { get; set; }
    }
}
