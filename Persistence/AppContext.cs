using Entities.Conferences;
using Entities.Helpers;
using Entities.Journals;
using Entities.Permissions;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UseCases.Interfaces;

namespace Persistence;

public class AppContext : DbContext, IDb
{
    private readonly IConfiguration _configuration;

    public AppContext(DbContextOptions<AppContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Journal> Journals { get; set; }
    public DbSet<JournalRecord> JournalRecords { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<UserGroupPermission> UserGroupPermissions { get; set; }
    public DbSet<UserInGroup> UserInGroups { get; set; }
    public DbSet<Conference> Conferences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(_configuration.GetConnectionString("MainConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IEntityTypeConfiguration<>).Assembly);
        SeedUser(modelBuilder);
    }

    private void SeedUser(ModelBuilder builder)
    {
        builder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "BanksAdmin",
                Password = HashPassword.Hash("BanksAdmin", "Ba1nk5Admin!"),
                Enabled = true,
                SysAdmin = true,
                Title = "مدیر سامانه"
            });
    }

    private void SeedUserGroup(ModelBuilder builder)
    {
        builder.Entity<UserGroup>().HasData(
            new UserGroup
            {
                Id = 1,
                Title = "مدیران"
            });
    }

    #region IDb Implementations

    DbSet<TEntity> IDb.Set<TEntity>()
    {
        return base.Set<TEntity>();
    }

    IQueryable<TEntity> IDb.Query<TEntity>()
    {
        return Set<TEntity>().OrderByDescending(i => i.Id);
    }

    int IDb.Save()
    {
        return base.SaveChanges();
    }

    #endregion
}