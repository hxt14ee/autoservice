using System;
using System.Collections.Generic;

namespace Autoservice.Models
{
    public class Client
    {
        public int ClientID { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ClientType { get; set; } 

        public virtual ICollection<Car> Cars { get; set; }
    }
}
