using System.Text.Json;
using ClassesData;
using ClassesData.Music;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DataBaseActions;

public class KostPostMusicContext : DbContext
{
    private static KostPostMusicContext _instance;


    public DbSet<Account> Accounts { get; set; }
    public DbSet<MusicData> MusicFiles { get; set; }

    public DbSet<Playlist> Playlists { get; set; }


    public KostPostMusicContext(DbContextOptions<KostPostMusicContext> options)
        : base(options)
    {
    }

    public KostPostMusicContext()
        : this(GetOptions())
    {
    }

    private static DbContextOptions<KostPostMusicContext> GetOptions()
    {
        return new DbContextOptionsBuilder<KostPostMusicContext>()
            .UseNpgsql("Host=localhost;Database=KostPostMusic;Username=postgres;Password=2025")
            .Options;
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (!optionsBuilder.IsConfigured)
    //     {
    //         optionsBuilder.UseNpgsql("Host=localhost;Database=KostPostMusic;Username=postgres;Password=2025");
    //         options => options.UseNodaTime());
    //
    //     }
    // }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (!optionsBuilder.IsConfigured)
    //     {
    //         optionsBuilder.UseNpgsql("Host=localhost;Database=KostPostMusic;Username=postgres;Password=2025",
    //             options => options.UseNodaTime());
    //     }
    // }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=KostPostMusic;Username=postgres;Password=2025",
                    options => options.UseNodaTime())
                .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                .EnableSensitiveDataLogging();
            
            
            // Enable legacy timestamp behavior
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        }   
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MusicData>(entity =>
        {
            entity.ToTable("music_data");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.AuthorID)
                .IsRequired();

            entity.Property(e => e.PlayCount)
                .HasDefaultValue(0);

            entity.Property(e => e.AuthorName)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(m => m.AuthorID);
        });
        
        modelBuilder.Entity<Playlist>()
            .Property(e => e.SongIds)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<int>>(v)
            );

        modelBuilder.Entity<Playlist>()
            .Property(e => e.SongAddedTimes)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<int, DateTime>>(v)
            );
        
                    
        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.ToTable("playlists");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasColumnType("text")
                .IsRequired();

            entity.Property(e => e.SongIds)
                .HasColumnName("song_ids")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<int>>(v))
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by")
                .HasColumnType("integer")
                .IsRequired();

            entity.Property(e => e.SongAddedTimes)
                .HasColumnName("song_added_times")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<int, DateTime>>(v))
                .IsRequired(false);
        });
    }
}