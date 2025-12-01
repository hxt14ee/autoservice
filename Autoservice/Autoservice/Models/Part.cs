using System;
using System.Collections.Generic;

namespace Autoservice.Models
{
    public class Part
    {
        public int PartID { get; set; }
        public string PartName { get; set; }
        public string PartNumber { get; set; }
        public string Manufacturer { get; set; }
        public decimal Price { get; set; }
        public virtual ICollection<OrderPart> OrderParts { get; set; }
    }
}
