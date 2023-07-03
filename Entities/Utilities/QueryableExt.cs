using System.Linq.Expressions;

namespace Entities.Utilities;

public static class QueryableExt
{
    public static TResult GetById<TEntity, TResult>(
        this IQueryable<TEntity> set,
        int id,
        Expression<Func<TEntity, TResult>> selector)
        where TEntity : class, IEntity
    {
        return set.Where(i => i.Id == id)
            .Select(selector).FirstOrDefault();
    }

    public static TResult? GetById<TEntity, TResult>(
        this IQueryable<TEntity> set,
        int? id,
        Expression<Func<TEntity, TResult>> selector)
        where TEntity : class, IEntity
        where TResult : class
    {
        TResult? byId;
        if (id.HasValue)
            byId = set.Where(i => i.Id == id)
                .Select(selector).FirstOrDefault();
        else
            byId = default;
        return byId;
    }

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

    public static IQueryable<T> Filter<T, TS>(
        this IQueryable<T> items,
        TS? value,
        Expression<Func<T, bool>> predicate)
        where TS : struct
    {
        return !value.HasValue ? items : items.Where(predicate);
    }

    public static IQueryable<T> Filter<T>(
        this IQueryable<T> items,
        string value,
        Expression<Func<T, bool>> predicate)
    {
        return string.IsNullOrWhiteSpace(value) ? items : items.Where(predicate);
    }

    public static IQueryable<T> FilterByIds<T>(
        this IQueryable<EntityTargetIdModel<T>> items,
        IList<int> ids)
    {
        if (ids == null)
            throw new Exception("ids == null");
        if (ids.Count > 20)
        {
            var ids2 = ids.Select((Func<int, int?>)(i => i)).ToList();
            return items
                .Where(
                    i => ids2.Contains(i.TargetId))
                .Select(i => i.Entity);
        }

        var id00 = ids.ElementAtOrDefault(0);
        var id01 = ids.ElementAtOrDefault(1);
        var id02 = ids.ElementAtOrDefault(2);
        var id03 = ids.ElementAtOrDefault(3);
        var id04 = ids.ElementAtOrDefault(4);
        var id05 = ids.ElementAtOrDefault(5);
        var id06 = ids.ElementAtOrDefault(6);
        var id07 = ids.ElementAtOrDefault(7);
        var id08 = ids.ElementAtOrDefault(8);
        var id09 = ids.ElementAtOrDefault(9);
        var id10 = ids.ElementAtOrDefault(10);
        var id11 = ids.ElementAtOrDefault(11);
        var id12 = ids.ElementAtOrDefault(12);
        var id13 = ids.ElementAtOrDefault(13);
        var id14 = ids.ElementAtOrDefault(14);
        var id15 = ids.ElementAtOrDefault(15);
        var id16 = ids.ElementAtOrDefault(16);
        var id17 = ids.ElementAtOrDefault(17);
        var id18 = ids.ElementAtOrDefault(18);
        var id19 = ids.ElementAtOrDefault(19);
        var count = ids.Count;
        if (true)
            ;
        IQueryable<T> queryable;
        switch (count)
        {
            case 0:
                queryable = items
                    .Where(i => false)
                    .Select(i => i.Entity);
                break;
            case 1:
                queryable = items
                    .Where(
                        i => i.TargetId == id00)
                    .Select(i => i.Entity);
                break;
            case 2:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01))
                    .Select(i => i.Entity);
                break;
            case 3:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02))
                    .Select(i => i.Entity);
                break;
            case 4:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03))
                    .Select(i => i.Entity);
                break;
            case 5:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04))
                    .Select(i => i.Entity);
                break;
            case 6:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05))
                    .Select(i => i.Entity);
                break;
            case 7:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06))
                    .Select(i => i.Entity);
                break;
            case 8:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06) | (i.TargetId == id07))
                    .Select(i => i.Entity);
                break;
            case 9:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06) | (i.TargetId == id07) | (i.TargetId == id08))
                    .Select(i => i.Entity);
                break;
            case 10:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06) | (i.TargetId == id07) | (i.TargetId == id08) |
                        (i.TargetId == id09))
                    .Select(i => i.Entity);
                break;
            case 11:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06) | (i.TargetId == id07) | (i.TargetId == id08) |
                        (i.TargetId == id09) | (i.TargetId == id10))
                    .Select(i => i.Entity);
                break;
            case 12:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06) | (i.TargetId == id07) | (i.TargetId == id08) |
                        (i.TargetId == id09) | (i.TargetId == id10) | (i.TargetId == id11))
                    .Select(i => i.Entity);
                break;
            case 13:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06) | (i.TargetId == id07) | (i.TargetId == id08) |
                        (i.TargetId == id09) | (i.TargetId == id10) | (i.TargetId == id11) |
                        (i.TargetId == id12))
                    .Select(i => i.Entity);
                break;
            case 14:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06) | (i.TargetId == id07) | (i.TargetId == id08) |
                        (i.TargetId == id09) | (i.TargetId == id10) | (i.TargetId == id11) |
                        (i.TargetId == id12) | (i.TargetId == id13))
                    .Select(i => i.Entity);
                break;
            case 15:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06) | (i.TargetId == id07) | (i.TargetId == id08) |
                        (i.TargetId == id09) | (i.TargetId == id10) | (i.TargetId == id11) |
                        (i.TargetId == id12) | (i.TargetId == id13) | (i.TargetId == id14))
                    .Select(i => i.Entity);
                break;
            case 16:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06) | (i.TargetId == id07) | (i.TargetId == id08) |
                        (i.TargetId == id09) | (i.TargetId == id10) | (i.TargetId == id11) |
                        (i.TargetId == id12) | (i.TargetId == id13) | (i.TargetId == id14) |
                        (i.TargetId == id15))
                    .Select(i => i.Entity);
                break;
            case 17:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06) | (i.TargetId == id07) | (i.TargetId == id08) |
                        (i.TargetId == id09) | (i.TargetId == id10) | (i.TargetId == id11) |
                        (i.TargetId == id12) | (i.TargetId == id13) | (i.TargetId == id14) |
                        (i.TargetId == id15) | (i.TargetId == id16))
                    .Select(i => i.Entity);
                break;
            case 18:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06) | (i.TargetId == id07) | (i.TargetId == id08) |
                        (i.TargetId == id09) | (i.TargetId == id10) | (i.TargetId == id11) |
                        (i.TargetId == id12) | (i.TargetId == id13) | (i.TargetId == id14) |
                        (i.TargetId == id15) | (i.TargetId == id16) | (i.TargetId == id17))
                    .Select(i => i.Entity);
                break;
            case 19:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06) | (i.TargetId == id07) | (i.TargetId == id08) |
                        (i.TargetId == id09) | (i.TargetId == id10) | (i.TargetId == id11) |
                        (i.TargetId == id12) | (i.TargetId == id13) | (i.TargetId == id14) |
                        (i.TargetId == id15) | (i.TargetId == id16) | (i.TargetId == id17) |
                        (i.TargetId == id18))
                    .Select(i => i.Entity);
                break;
            case 20:
                queryable = items
                    .Where(i =>
                        (i.TargetId == id00) | (i.TargetId == id01) | (i.TargetId == id02) |
                        (i.TargetId == id03) | (i.TargetId == id04) | (i.TargetId == id05) |
                        (i.TargetId == id06) | (i.TargetId == id07) | (i.TargetId == id08) |
                        (i.TargetId == id09) | (i.TargetId == id10) | (i.TargetId == id11) |
                        (i.TargetId == id12) | (i.TargetId == id13) | (i.TargetId == id14) |
                        (i.TargetId == id15) | (i.TargetId == id16) | (i.TargetId == id17) |
                        (i.TargetId == id18) | (i.TargetId == id19))
                    .Select(i => i.Entity);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (true)
            ;
        return queryable;
    }

    public static int? FirstId<TEntity>(this IQueryable<TEntity> items) where TEntity : class, IEntity
    {
        return items
            .OrderBy(i => i.Id).Select(i => new
            {
                i.Id
            }).FirstOrDefault()?.Id;
    }

    public static int? NextId<TEntity>(this IQueryable<TEntity> items, int? id) where TEntity : class, IEntity
    {
        if (!id.HasValue)
            return items.FirstId();
        return items.Where(i => i.Id > id.Value)
            .OrderBy(i => i.Id).Select(i => new
            {
                i.Id
            }).FirstOrDefault()?.Id ?? items.FirstId();
    }
}

public class EntityTargetIdModel<T>
{
    public T Entity { get; set; }

    public int? TargetId { get; set; }
}