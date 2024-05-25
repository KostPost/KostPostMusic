using ClassesData;
using Microsoft.EntityFrameworkCore;

namespace DataBaseActions;

public class UserService
{
    private readonly KostPostMusicContext _dbContext;

    public UserService(KostPostMusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    // public async Task<UserAccount?> GetUserByUsername(string username)
    // {
    //     return await _dbContext.Accounts.OfType<UserAccount>().FirstOrDefaultAsync(u => u.Username == username);
    // }
    
    public async Task<UserAccount?>? GetUserByUsername(string username)
    {
        return _dbContext.UserAccounts.FirstOrDefault(u => u.Username == username);
    }

    public async Task<bool> AddUserAccount(UserAccount userAccount)
    {
        var existingUser = await _dbContext.Accounts.FirstOrDefaultAsync(u => u.Username == userAccount.Username);

        if (existingUser != null)
        {
            return false;
        }

        await _dbContext.Accounts.AddAsync(userAccount);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}


public enum AuthenticationResult
{
    Success,
    UserNotFound,
    IncorrectPassword
}

public enum RegistrationResult
{
    Success,
    UserAlreadyExists,
    Error
}