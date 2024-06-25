using System;
using System.Linq;
using System.Threading.Tasks;
using ClassesData;
using ClassesData.Music;
using Microsoft.EntityFrameworkCore;
using MusicAPI;

namespace DataBaseActions.Music;

public class MusicService
{
    private readonly KostPostMusicContext _dbContext;
    private readonly AzureBlobs _azureBlobs;

    public MusicService(KostPostMusicContext dbContext, AzureBlobs azureBlobs)
    {
        _dbContext = dbContext;
        _azureBlobs = azureBlobs;
    }

    public async Task<(bool Success, string Message)> AddMusicFileAsync(string musicName, string filePath,
        Account account, TimeSpan duration)
    {
        bool azureSuccess = false;
        bool sqlSuccess = false;

        if (_dbContext.MusicFiles.Any(m => m.FileName == musicName))
        {
            return (false, $"A music file with the name '{musicName}' already exists.");
        }
        
        try
        {
            await _azureBlobs.UploadBlobAsync(filePath, musicName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Azure Error: {ex.Message}");
            return (false, "Failed to upload file to Azure storage.");
        }
        
        azureSuccess = true;

        if (azureSuccess)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var music = new MusicData
                {
                    FileName = musicName,
                    AuthorName = account.Username,
                    AuthorID = account.Id,
                    Duration = duration  // Add the duration here
                };

                _dbContext.MusicFiles.Add(music);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                sqlSuccess = true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"PostgreSQL Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // If SQL fails, try to delete the uploaded blob from Azure
                try
                {
                    await _azureBlobs.DeleteBlobAsync(musicName);
                }
                catch (Exception deleteEx)
                {
                    Console.WriteLine($"Failed to delete Azure blob after SQL error: {deleteEx.Message}");
                }

                return (false,
                    "Failed to save data to the database. The uploaded file has been removed from Azure storage.");
            }
        }

        if (azureSuccess && sqlSuccess)
        {
            return (true, "Music file successfully added to both Azure storage and the database.");
        }
        else
        {
            return (false, "An unexpected error occurred.");
        }
    }
}