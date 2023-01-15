using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using File = Entities.File;

namespace Persistence;

public class FileContext : DbContext
{
    private readonly IConfiguration Configuration;

    public FileContext(DbContextOptions<FileContext> options, IConfiguration configuration)
        : base(options)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(Configuration.GetConnectionString("FileConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FileConfiguration());
    }

    public DbSet<File> Files { get; set; }
}