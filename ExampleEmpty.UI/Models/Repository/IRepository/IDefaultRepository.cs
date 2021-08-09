using System.Collections.Generic;

namespace ExampleEmpty.UI.Models.Repository.IRepository
{
    public interface IDefaultRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Get();
        TEntity Get(int id);

        TEntity Create(TEntity entityToCreate);
        TEntity Remove(TEntity entityToRemove);
        TEntity Remove(int entityToRemoveId);
        void Update(TEntity entityToUpdate);
    }
}
