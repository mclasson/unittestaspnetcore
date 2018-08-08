using System;
using Demo.Definitions;

namespace Demo.Models
{
    public class Customer : ICustomer
    {
        public string CustomerName {get; set; }
        public int Id {get; set; }
        public DateTime CreatedOn {get; set; }
    }
}
