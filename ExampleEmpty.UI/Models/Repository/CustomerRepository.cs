using ExampleEmpty.UI.Models.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;

namespace ExampleEmpty.UI.Models.Repository
{
    //public class CustomerRepository : ICustomerRepository
    //{
    //    private readonly List<Customer> _listOfCustomer;
    //    public CustomerRepository()
    //    {
    //        _listOfCustomer = new(Customers);
    //    }
    //    private static List<Customer> Customers
    //    {
    //        get
    //        {
    //            return new()
    //            {
    //                new(1, "Tom Smith", "Jallah Town", Gender.Male),
    //                new(2, "Sara Collins", "Sinkor", Gender.Female),
    //                new(3, "Test User 1", "Bye Pass", Gender.Unknown)
    //            };
    //        }
    //    }
    //    public Customer Create(Customer entityToCreate)
    //    {
    //        entityToCreate.CustomerId = _listOfCustomer.Max(x => x.CustomerId) + 1;
    //        _listOfCustomer.Add(entityToCreate);
    //        return entityToCreate;
    //    }
    //    public IEnumerable<Customer> Get()
    //    {
    //        return _listOfCustomer.ToList();
    //    }
    //    public Customer Get(int id)
    //    {
    //        return _listOfCustomer.FirstOrDefault(c => c.CustomerId == id);
    //    }

    //    public Customer Remove(Customer entityToRemove)
    //    {
    //        var found = _listOfCustomer.FirstOrDefault(c => c.CustomerId == entityToRemove.CustomerId);
    //        if (found != null)
    //        {
    //            _listOfCustomer.Remove(found);
    //            return found;
    //        }
    //        return null;
    //    }

    //    public void Update(Customer entityToUpdate)
    //    {
    //        var customerFound = Get(entityToUpdate.CustomerId);
    //        if (customerFound != null)
    //        {
    //            customerFound.Name = entityToUpdate.Name;
    //            customerFound.Address = entityToUpdate.Address;
    //            customerFound.Gender = entityToUpdate.Gender;
    //        }
    //    }
    //}
}
