using System.Linq.Expressions;
using DAL.Entities;

namespace DAL.IRepositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetById(int id);
    Task<List<TEntity>> GetAll();
    Task<List<TEntity>> GetWithIncludesAsync(params Expression<Func<TEntity, object>>[] includeProperties);

    Task<ResponsePage<TEntity>> GetWithIncludesPaginatedAsync(SearchParams searchParams,
            params Expression<Func<TEntity, object>>[] includeProperties);

    Task<ResponsePage<TEntity>> GetFilteredWithIncludesPaginatedAsync(Expression<Func<TEntity, bool>>[] filters,
            SearchParams searchParams, params Expression<Func<TEntity, object>>[] includeProperties);

    Task<List<TEntity>> GetFilteredWithIncludesAsync(Expression<Func<TEntity, bool>>? filter = null,
        params Expression<Func<TEntity, object>>[] includeProperties);

    Task<TEntity?> Find(Expression<Func<TEntity, bool>> predicate);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task SaveChangesAsync();
}