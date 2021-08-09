namespace ExampleEmpty.UI.Models.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ICustomerRepository CustomerRepository { get; }
      
    }
}
