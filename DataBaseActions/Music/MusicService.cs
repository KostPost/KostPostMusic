using ClassesData.Music;
using Microsoft.EntityFrameworkCore;
using MusicAPI;

namespace DataBaseActions.Music;

public class MusicService
{
    private readonly KostPostMusicContext _dbContext;
    private readonly AzureBlobs _azureBlobs;
    private readonly Album _album;

    public MusicService(KostPostMusicContext dbContext, AzureBlobs azureBlobs)
    {
        _dbContext = dbContext;
        _azureBlobs = azureBlobs;
    }

    public async Task AddAlbumAsync(string musicName, string albumName, string authors, IEnumerable<string> filePaths)
    {
        var album = new Album
        {
            Name = albumName
        };

        _dbContext.Albums.Add(album);
        _dbContext.SaveChangesAsync();

        var authorList = ParseAuthors(authors);

        foreach (var filePath in filePaths)
        {
            var fileName = Path.GetFileName(filePath);
            _azureBlobs.UploadBlobAsync(filePath, fileName);

            var music = new MusicFile
            {
                FileName = fileName,
                AlbumId = album.Id,
                Authors = authorList
            };

            _dbContext.MusicFiles.Add(music);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task AddMusicFileAsync(string musicName, string albumName, string authors, string filePath)
    {
        var album = _dbContext.Albums.FirstOrDefault(a => a.Name == albumName);
        if (album == null)
        {
            album = new Album { Name = albumName };
            _dbContext.Albums.Add(album);
        }

        var authorList = ParseAuthors(authors);

        var fileName = Path.GetFileName(filePath);
        _azureBlobs.UploadBlobAsync(filePath, fileName);

        var music = new MusicFile
        {
            FileName = fileName,
            AlbumId = album.Id,
            Authors = authorList
        };

        _dbContext.MusicFiles.Add(music);
        _dbContext.SaveChangesAsync();
    }

    private ICollection<MusicAuthor> ParseAuthors(string authors)
    {
        var authorList = new List<MusicAuthor>();
        var authorNames = authors.Split(',');

        foreach (var authorName in authorNames)
        {
            authorList.Add(new MusicAuthor { Name = authorName.Trim() });
        }

        return authorList;
    }
}