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

    public async Task<Account?> GetUserByUsername(string username)
    {
        return await _dbContext.Accounts.FirstOrDefaultAsync(u => u.Username == username).ConfigureAwait(false);
    }

    public async Task<bool> AddUserAccount(Account account)
    {
        try
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            var existingUser = await GetUserByUsername(account.Username).ConfigureAwait(false);
            if (existingUser != null)
            {
                return false;
            }

            await _dbContext.Accounts.AddAsync(account).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding user account: {ex.Message}");
            return false;
        }
    }
}


public enum AuthenticationResult
{
    Success,
    UserNotFound,
    IncorrectPassword,
    Error
}

public enum RegistrationResult
{
    Success,
    UserAlreadyExists,
    InvalidInput,
    DatabaseError,
    Error
}