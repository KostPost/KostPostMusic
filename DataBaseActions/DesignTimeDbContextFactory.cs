using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataBaseActions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UserAccountDbContext>
{
    public UserAccountDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<UserAccountDbContext>();
        builder.UseNpgsql("Host=localhost;Database=KostPostMusic;Username=postgres;Password=2025;");

        return new UserAccountDbContext(builder.Options);
    }
}