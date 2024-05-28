using ClassesData;
using Microsoft.EntityFrameworkCore;

namespace DataBaseActions;

public class KostPostMusicContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }


    public KostPostMusicContext(DbContextOptions<KostPostMusicContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=KostPostMusic;Username=postgres;Password=2025");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("accounts");

            entity.HasKey(e => e.Id).HasName("PK_Accounts");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("password");

            entity.Property(e => e.AccountType)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("account_type");

            entity.Property(e => e.AdminLevel).HasColumnName("admin_level");
        });
    }
}