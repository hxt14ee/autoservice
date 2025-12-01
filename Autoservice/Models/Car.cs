using System;
using System.Collections.Generic;

namespace Autoservice.Models
{
    public class Car
    {
        public int CarID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }
        public string VIN { get; set; }
        public int ClientID { get; set; }

        // навигация
        public virtual Client Client { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
