using ClassesData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataBaseActions;

public class AccountsDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }

    public AccountsDbContext(DbContextOptions<AccountsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .ToTable("accounts")
            .HasDiscriminator<string>("Role")
            .HasValue<AdminAccount>("Admin")
            .HasValue<UserAccount>("User");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=KostPostMusic;Username=postgres;Password=2025");
    }
}