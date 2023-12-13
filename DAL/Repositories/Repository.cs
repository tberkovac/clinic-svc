using System.Linq.Expressions;
using DAL.Data;
using DAL.Entities;
using DAL.IRepositories;
using Microsoft.EntityFrameworkCore;


namespace DAL.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _dbContext;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TEntity>> GetAll()
    {   
        return await _dbContext.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity?> GetById(int id)
    {
        return await _dbContext.Set<TEntity>().FindAsync(id);
    }       

    public async Task<TEntity?> Find(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbContext.Set<TEntity>().Where(predicate).FirstOrDefaultAsync();
    }

    public void Add(TEntity entity)
    {
        _dbContext.Set<TEntity>().Add(entity);
    }

    public void Update(TEntity entity)
    {
        _dbContext.Set<TEntity>().Update(entity);
    }

    public void Remove(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
    }

    public Task SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public async Task<List<TEntity>> GetWithIncludesAsync(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return query.ToListAsync();
    }

    public async Task<List<TEntity>> GetFilteredWithIncludesAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.ToListAsync();
    }

    public async Task<ResponsePage<TEntity>> GetWithIncludesPaginatedAsync(SearchParams searchParams, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();

        var size = await query.CountAsync();

        query = query.Skip(searchParams.Page * searchParams.PageSize)
        .Take(searchParams.PageSize);

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        var data = query.ToListAsync();

        var responsePage = new ResponsePage<TEntity>
        {
            Data = data,
            NumberOfElements = size,
            Page = searchParams.Page,
            PageSize = searchParams.PageSize
        };

        return responsePage;
    }

    public async Task<ResponsePage<TEntity>> GetFilteredWithIncludesPaginatedAsync(List<Expression<Func<TEntity, bool>>> filters, SearchParams searchParams, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();

        foreach (var filter in filters)
        {
            query = query.Where(filter);
        }

        var size = await query.CountAsync();

        query = query.Skip(searchParams.Page * searchParams.PageSize)
        .Take(searchParams.PageSize);

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        var data = await query.ToListAsync();

        var responsePage = new ResponsePage<TEntity>
        {
            Data = data,
            NumberOfElements = size,
            Page = searchParams.Page,
            PageSize = searchParams.PageSize
        };

        return responsePage;
    }
}