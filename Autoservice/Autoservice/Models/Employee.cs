using System;
using System.Collections.Generic;

namespace Autoservice.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public decimal Salary { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
