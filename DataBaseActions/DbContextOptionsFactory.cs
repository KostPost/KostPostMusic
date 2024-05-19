using Microsoft.EntityFrameworkCore;

namespace DataBaseActions;

public class DbContextOptionsFactory
{
    public string ConnectionString { get; }

    public DbContextOptionsFactory(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public DbContextOptions<UserAccountDbContext> BuildDbContextOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserAccountDbContext>();
        optionsBuilder.UseNpgsql(ConnectionString);
        return optionsBuilder.Options;
    }
}