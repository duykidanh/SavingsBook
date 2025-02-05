using System.Linq.Expressions;
using MongoDB.Driver;
using SavingsBook.Domain.Common;

namespace SavingsBook.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : AuditedAggregateRoot<Guid>
{
    private readonly IMongoCollection<T> _dbCollection;
    private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;

    public Repository(IMongoDatabase database, string collectionName)
    {
        _dbCollection = database.GetCollection<T>(collectionName);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var filter = _filterBuilder.And(_filterBuilder.Eq(entity => entity.Id, id), ExcludeSoftDelete());

        var update = Builders<T>.Update
            .Set(entity => entity.IsDeleted, true)
            .Set(entity => entity.DeletionTime, DateTime.UtcNow);

        var result = await _dbCollection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task DeleteManyAsync(IEnumerable<T> entities)
    {
        var ids = entities.Select(e => e.Id);
        var combinedFilter = _filterBuilder.And(_filterBuilder.In(e => e.Id, ids), ExcludeSoftDelete());
        await _dbCollection.DeleteManyAsync(combinedFilter);
    }

    public async Task<T> InsertAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        entity.ConcurrencyStamp = Guid.NewGuid().ToString();
        entity.CreationTime = DateTime.Now;

        await _dbCollection.InsertOneAsync(entity);
        return entity;
    }

    public async Task InsertManyAsync(IEnumerable<T> entities)
    {
        await _dbCollection.InsertManyAsync(entities.Select(e =>
        {
            e.ConcurrencyStamp = Guid.NewGuid().ToString();
            e.CreationTime = DateTime.Now;
            return e;
        }));
    }

    public async Task<T> UpdateAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(existEntity => existEntity.Id, entity.Id),
            _filterBuilder.Eq(existEntity => existEntity.ConcurrencyStamp, entity.ConcurrencyStamp)
        );
        var update = Builders<T>.Update
            .Set(e => e.ConcurrencyStamp, Guid.NewGuid().ToString())
            .Set(e => e.LastModificationTime, DateTime.Now);

        entity.ConcurrencyStamp = Guid.NewGuid().ToString();
        await _dbCollection.UpdateOneAsync(filter, update);

        return entity;
    }

    public async Task<T> GetAsync(Guid id)
    {
        var combinedFilter = _filterBuilder.And(_filterBuilder.Eq(entity => entity.Id, id), ExcludeSoftDelete());
        return await _dbCollection.Find(combinedFilter).FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetListAsync()
    {
        return await _dbCollection.Find(ExcludeSoftDelete()).ToListAsync();
    }

    public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate)
    {
        var combinedFilter = _filterBuilder.And(predicate, ExcludeSoftDelete());
        return await _dbCollection.Find(combinedFilter).ToListAsync();
    }

    public async Task<long> CountAsync(Expression<Func<T, bool>> predicate)
    {
        var combinedFilter = _filterBuilder.And(predicate, ExcludeSoftDelete());
        return await _dbCollection.Find(combinedFilter).CountDocumentsAsync();
    }

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        var combinedFilter = _filterBuilder.And(predicate, ExcludeSoftDelete());
        return await _dbCollection.Find(combinedFilter).FirstOrDefaultAsync();
    }

    public async Task<IQueryable<T>> GetQueryableAsync()
    {
        return _dbCollection.AsQueryable().Where(x => !x.IsDeleted);
    }

    private FilterDefinition<T> ExcludeSoftDelete()
    {
        return _filterBuilder.Eq(x => x.IsDeleted, false);
    }
}