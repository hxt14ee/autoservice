using System;

namespace Autoservice.Models
{
    public class OrderPart
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public int PartID { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }

        public virtual Order Order { get; set; }
        public virtual Part Part { get; set; }
    }
}
