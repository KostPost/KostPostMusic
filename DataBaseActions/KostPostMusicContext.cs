using ClassesData;
using ClassesData.Music;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataBaseActions;

public class KostPostMusicContext : DbContext
{
    private static KostPostMusicContext _instance;
    public DbSet<Album> Albums { get; set; }


    public DbSet<Account> Accounts { get; set; }
    public DbSet<MusicFile> MusicFiles { get; set; }
    public DbSet<MusicAuthor> MusicAuthors { get; set; }

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
    modelBuilder.Entity<Album>(entity =>
    {
        entity.ToTable("albums");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasColumnName("id");

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("name");

        entity.HasMany(e => e.MusicFiles)
            .WithOne(e => e.Album)
            .HasForeignKey(e => e.AlbumId)
            .IsRequired();
    });

    modelBuilder.Entity<MusicFile>(entity =>
    {
        entity.ToTable("music_files");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasColumnName("id");

        entity.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("file_name");

        entity.Property(e => e.AlbumId)
            .IsRequired()
            .HasColumnName("album_id");

        entity.HasMany(e => e.Authors)
            .WithMany(e => e.MusicFiles)
            .UsingEntity<Dictionary<string, object>>(
                "music_file_authors",
                j => j.HasOne<MusicAuthor>()
                    .WithMany(a => a.MusicFiles)
                    .HasForeignKey("music_author_id")
                    .HasConstraintName("FK_music_file_authors_music_author_id")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<MusicFile>()
                    .WithMany(m => m.Authors)
                    .HasForeignKey("music_file_id")
                    .HasConstraintName("FK_music_file_authors_music_file_id")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("music_file_id", "music_author_id");
                    j.ToTable("music_file_authors");
                }
            );
    });

    modelBuilder.Entity<MusicAuthor>(entity =>
    {
        entity.ToTable("music_authors");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasColumnName("id");

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("name");

        entity.HasMany(e => e.MusicFiles)
            .WithMany(e => e.Authors)
            .UsingEntity<Dictionary<string, object>>(
                "music_file_authors",
                j => j.HasOne<MusicAuthor>()
                    .WithMany(a => a.MusicFiles)
                    .HasForeignKey("music_author_id")
                    .HasConstraintName("FK_music_file_authors_music_author_id")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<MusicFile>()
                    .WithMany(m => m.Authors)
                    .HasForeignKey("music_file_id")
                    .HasConstraintName("FK_music_file_authors_music_file_id")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("music_file_id", "music_author_id");
                    j.ToTable("music_file_authors");
                }
            );
    });
}
}