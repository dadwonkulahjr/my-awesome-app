using ExampleEmpty.UI.Models.Data;
using System.Collections.Generic;
using System.Linq;


namespace ExampleEmpty.UI.Models.Repository.IRepository
{
    public class CustomerSQLRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _appDbContext;
        public CustomerSQLRepository(ApplicationDbContext dbContext)
        {
            _appDbContext = dbContext;
          
        }
        public Customer Create(Customer entityToCreate)
        {
            _appDbContext.Customers.Add(entityToCreate);
            _appDbContext.SaveChanges();
            return entityToCreate;
        }
        public IEnumerable<Customer> Get()
        {
            return _appDbContext.Customers.ToList();
        }
        public Customer Get(int id)
        {
            return _appDbContext.Customers.Find(id);
        }
        public Customer Remove(Customer entityToRemove)
        {
            var findEntity = _appDbContext.Customers.Find(entityToRemove.CustomerId);
            if (findEntity != null)
            {
                _appDbContext.Customers.Remove(findEntity);
                _appDbContext.SaveChanges();
                return entityToRemove;
            }

            return null;
        }
        public Customer Remove(int entityToRemove)
        {
            var found = _appDbContext.Customers.Find(entityToRemove);
            if (found != null)
            {
                _appDbContext.Customers.Remove(found);
                _appDbContext.SaveChanges();
                return found;

            }
            return null;
        }
        public void Update(Customer entityToUpdate)
        {
            var findCustomer = _appDbContext.Customers.FirstOrDefault(c => c.CustomerId == entityToUpdate.CustomerId);

            if (findCustomer != null)
            {
                findCustomer.Name = entityToUpdate.Name;
                findCustomer.Address = entityToUpdate.Address;
                findCustomer.Gender = entityToUpdate.Gender;
                if (findCustomer.PhotoPath != null)
                {
                    if (entityToUpdate.PhotoPath != null)
                    {
                        findCustomer.PhotoPath = entityToUpdate.PhotoPath;
                    }
                }
                _appDbContext.SaveChanges();
            }
        }
    }
}
