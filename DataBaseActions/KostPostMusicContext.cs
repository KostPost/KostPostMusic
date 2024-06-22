using ClassesData;
using ClassesData.Music;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataBaseActions;

public class KostPostMusicContext : DbContext
{
    private static KostPostMusicContext _instance;


    public DbSet<Account> Accounts { get; set; }
    public DbSet<MusicData> MusicFiles { get; set; }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=KostPostMusic;Username=postgres;Password=2025");
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

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.ToTable("playlists");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.OwnerId)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(p => p.OwnerId);

            entity.HasMany(p => p.Songs)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "PlaylistSongs",
                    j => j
                        .HasOne<MusicData>()
                        .WithMany()
                        .HasForeignKey("SongId"),
                    j => j
                        .HasOne<Playlist>()
                        .WithMany()
                        .HasForeignKey("PlaylistId"),
                    j =>
                    {
                        j.HasKey("PlaylistId", "SongId");
                        j.ToTable("playlist_songs");
                    });
        });
    }
}