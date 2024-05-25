// using ClassesData;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace DataBaseActions;
//
// using Microsoft.EntityFrameworkCore;
//
// public class UserAccountDbContext : DbContext
// {
//     public DbSet<UserAccount> UserAccounts { get; set; }
//     public DbSet<AdminAccount> AdminAccounts { get; set; }
//
//     public UserAccountDbContext(DbContextOptions<UserAccountDbContext> options) : base(options)
//     {
//     }
//     
//     public void ConfigureServices(IServiceCollection services)
//     {
//         var connectionString = "Host=localhost;Database=KostPostMusic;Username=postgres;Password=2025;";
//         //var dbContextOptionsFactory = new DbContextOptionsFactory(connectionString);
//         //var dbContextOptions = dbContextOptionsFactory.BuildDbContextOptions();
//
//         services.AddDbContext<UserAccountDbContext>(options => options.UseNpgsql(connectionString));
//     }
//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         modelBuilder.Entity<Account>()
//             .ToTable("accounts")
//             .Property(a => a.Id)
//             .HasColumnName("id");
//
//         modelBuilder.Entity<Account>()
//             .HasDiscriminator<string>("account_type")
//             .HasValue<UserAccount>("User")
//             .HasValue<AdminAccount>("Admin");
//
//         // Remove the following lines:
//         // modelBuilder.Entity<UserAccount>(entity =>
//         // {
//         //     entity.HasKey(e => e.Id);
//         //     entity.Property(e => e.Username).IsRequired();
//         // });
//     }
//
//     public async Task<UserAccount?> FindByUsernameAsync(string username)
//     {
//         return await UserAccounts
//             .FirstOrDefaultAsync(u => u.Username == username);
//     }
//
//     public async Task AddUserAccountAsync(UserAccount userAccount)
//     {
//         await UserAccounts.AddAsync(userAccount);
//         await SaveChangesAsync();
//     }
//     
//     
//     
//
//
// }