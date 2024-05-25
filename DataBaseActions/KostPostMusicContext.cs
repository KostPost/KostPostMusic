using ClassesData;
using Microsoft.EntityFrameworkCore;

namespace DataBaseActions;

public class KostPostMusicContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<UserAccount> UserAccounts { get; set; } 

    
    public KostPostMusicContext(DbContextOptions<KostPostMusicContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=KostPostMusic;Username=postgres;Password=2025");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .HasDiscriminator<AccountType>("AccountType")
            .HasValue<UserAccount>(AccountType.User)
            .HasValue<AdminAccount>(AccountType.Admin);
    }
}