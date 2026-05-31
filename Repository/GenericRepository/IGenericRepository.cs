using System.Linq.Expressions;

namespace WebApplication1.Repository.GenericRepository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
         Task<IEnumerable<TEntity>> GetAllAsync();
         Task<(IEnumerable<TEntity> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize);

        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

         Task<TEntity?> GetByIdAsync(int id);

         Task AddAsync(TEntity entity);

         TEntity Update(TEntity entity);

         TEntity Delete(TEntity entity);

         Task<bool> SaveChangesAsync();
    }
}
