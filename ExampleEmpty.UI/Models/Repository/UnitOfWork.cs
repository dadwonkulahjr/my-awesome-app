using ExampleEmpty.UI.Models.Data;
using ExampleEmpty.UI.Models.Repository.IRepository;

namespace ExampleEmpty.UI.Models.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICustomerRepository CustomerRepository => throw new System.NotImplementedException();
    }
}
