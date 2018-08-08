using System;

namespace Demo.Definitions
{
    public interface IRepository<T> where T : IBusinessItem
    {
        T GetById(int id);
    }
}
