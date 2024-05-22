using ClassesData.Music;

namespace DataBaseActions.Music;

public class MusicFileService
{
    private readonly MusicFileDbContext _context;
    private readonly string _musicFilesDirectory;

    public MusicFileService(MusicFileDbContext context, string musicFilesDirectory)
    {
        _context = context;
        _musicFilesDirectory = musicFilesDirectory;
    }

    public async Task<int> SaveMusicFile(string fileName, Stream fileStream)
    {
        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
        string filePath = Path.Combine(_musicFilesDirectory, uniqueFileName);

        using (var outputStream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(outputStream);
        }

        var musicFile = new MusicFile
        {
            FileName = fileName,
            FilePath = filePath,
        };

        _context.MusicFiles.Add(musicFile);
        await _context.SaveChangesAsync();

        return musicFile.Id;
    }

    public async Task<Stream> GetMusicFileById(int fileId)
    {
        var musicFile = await _context.MusicFiles.FindAsync(fileId);
        if (musicFile == null)
        {
            throw new FileNotFoundException("Music file not found.");
        }

        return new FileStream(musicFile.FilePath, FileMode.Open);
    }
}