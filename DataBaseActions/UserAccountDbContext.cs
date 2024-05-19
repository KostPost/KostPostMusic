using Microsoft.Extensions.DependencyInjection;

namespace DataBaseActions;

using Microsoft.EntityFrameworkCore;

public class UserAccountDbContext : DbContext
{
    public DbSet<UserAccount> UserAccounts { get; set; }
    
    public UserAccountDbContext(DbContextOptions<UserAccountDbContext> options) : base(options)
    {
    }

}