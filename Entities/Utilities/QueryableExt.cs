using System.Linq.Expressions;

namespace Entities
{
    public static class QueryableExt
    {
        public static IQueryable<TEntity> FilterById<TEntity>(this IQueryable<TEntity> items, int? id)
            where TEntity : class, IEntity
        {
            return items.Filter(id, i => i.Id == id.Value);
        }

        public static TEntity GetById<TEntity>(this IQueryable<TEntity> items, int? id)
            where TEntity : class, IEntity
        {
            return items.FirstOrDefault(i => i.Id == id.Value);
        }

        public static IQueryable<TEntity> IgnoreById<TEntity>(this IQueryable<TEntity> items, int? id)
            where TEntity : class, IEntity
        {
            return items.Filter(id, i => i.Id != id.Value);
        }

        public static IQueryable<TEntity> SkipId<TEntity>(this IQueryable<TEntity> items, int? id)
            where TEntity : class, IEntity
        {
            return items.Filter(id, i => id.Value < i.Id);
        }

        public static IQueryable<T> Filter<T, TS>(this IQueryable<T> items, TS? value,
            Expression<Func<T, bool>> predicate) where TS : struct
        {
            return !value.HasValue ? items : items.Where(predicate);
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> items, string value,
            Expression<Func<T, bool>> predicate)
        {
            return string.IsNullOrWhiteSpace(value) ? items : items.Where(predicate);
        }
    }
}