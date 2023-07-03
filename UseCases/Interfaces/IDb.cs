using Entities;
using Microsoft.EntityFrameworkCore;

namespace UseCases.Interfaces;

public interface IDb
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    IQueryable<TEntity> Query<TEntity>() where TEntity : class, IEntity;

    int Save();
}