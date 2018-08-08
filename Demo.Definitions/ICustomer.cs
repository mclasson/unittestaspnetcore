using System;

namespace Demo.Definitions
{
    public interface ICustomer : IBusinessItem
    {
        string CustomerName {get; set;}
    }
}
