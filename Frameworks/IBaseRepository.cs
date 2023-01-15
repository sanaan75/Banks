using System.Linq.Expressions;

public interface IBaseRepository<T>
{
    IQueryable<T> FindAll(bool trackChanges);

    IQueryable<T> Filter(Expression<Func<T, bool>> expression,
        bool trackChanges);

    void Update(T entity);

    T GetById(int id);

    IEnumerable<T> ListAll();

    IQueryable<T> GetAll();

    IEnumerable<T> FindList(Expression<Func<T, bool>> expression);

    IQueryable<T> Find(Expression<Func<T, bool>> expression);

    T Add(T entity);

    void AddRange(IEnumerable<T> entities);

    void Remove(T entity);

    void RemoveRange(IEnumerable<T> entities);
}