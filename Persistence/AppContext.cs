using Entities.Journals;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Entities.Helpers;
using Entities.Permissions;
using Microsoft.Extensions.Configuration;

namespace Persistence
{
    public class AppContext : DbContext
    {
        private readonly IConfiguration Configuration;

        public AppContext(DbContextOptions<AppContext> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("MainConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IEntityTypeConfiguration<>).Assembly);
            SeedUser(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Journal> Journals { get; set; }
        public DbSet<JournalRecord> JournalRecords { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserGroupPermission> UserGroupPermissions { get; set; }
        public DbSet<UserInGroup> UserInGroups { get; set; }

        private void SeedUser(ModelBuilder builder)
        {
            builder.Entity<User>().HasData(
                new User()
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
    }
}