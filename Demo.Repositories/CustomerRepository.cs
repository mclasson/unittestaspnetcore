using System;
using Demo.Definitions;
using Demo.Models;

namespace Demo.Repositories
{

    public class CustomerRepository : IRepository<ICustomer>
    {
        public ICustomer GetById(int id)
        {
            //Simulate db access
            return new Customer{Id = 1, CustomerName = "Acme", CreatedOn = DateTime.Now};
        }
    }

}