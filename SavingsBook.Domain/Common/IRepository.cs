using System.Linq.Expressions;

namespace SavingsBook.Domain.Common
{
    public interface IRepository<T> where T : AuditedAggregateRoot<Guid>
    {
        Task<T> GetAsync(Guid id);
        Task<List<T>> GetListAsync();
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate);
        Task<long> CountAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T> InsertAsync(T entity);
        Task InsertManyAsync(IEnumerable<T> entities);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task DeleteManyAsync(IEnumerable<T> entities);
        Task<IQueryable<T>> GetQueryableAsync();
    }
}