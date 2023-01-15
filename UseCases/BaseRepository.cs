using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;


namespace Services;

public abstract class BaseRepository<TContext, T> : IBaseRepository<T> where T : class where TContext : DbContext
{
    protected TContext _context;

    public BaseRepository(TContext context) => this._context = context;

    public IQueryable<T> FindAll(bool trackChanges) =>
        !trackChanges
            ? _context.Set<T>()
                .AsNoTracking()
            : _context.Set<T>();

    public IQueryable<T> Filter(Expression<Func<T, bool>> expression,
        bool trackChanges) =>
        !trackChanges
            ? _context.Set<T>()
                .Where(expression)
                .AsNoTracking()
            : _context.Set<T>()
                .Where(expression);

    public void Create(T entity) => _context.Set<T>().Add(entity);

    public void Update(T entity) => _context.Set<T>().Update(entity);

    public T Add(T entity)
    {
        return _context.Set<T>().Add(entity).Entity;
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _context.Set<T>().AddRange(entities);
    }

    public IEnumerable<T> FindList(Expression<Func<T, bool>> expression)
    {
        return _context.Set<T>().Where(expression);
    }

    public IQueryable<T> Find(Expression<Func<T, bool>> expression)
    {
        return _context.Set<T>().Where(expression).AsQueryable();
    }

    public IEnumerable<T> ListAll()
    {
        return _context.Set<T>().ToList();
    }

    public T GetById(int id)
    {
        return _context.Set<T>().Find(id);
    }

    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }

    public IQueryable<T> GetAll()
    {
        return _context.Set<T>().AsQueryable();
    }

}