using ClassesData;
using Microsoft.EntityFrameworkCore;

namespace DataBaseActions;

public class UserService
{
    private readonly UserAccountDbContext _context;

    public UserService(UserAccountDbContext context)
    {
        _context = context;
    }

    public async Task<UserAccount?> GetUserByUsername(string username)
    {
        return await _context.UserAccounts.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> AddUserAccount(UserAccount userAccount)
    {
        var existingUser = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Username == userAccount.Username);

        if (existingUser != null)
        {
            return false;
        }

        await _context.UserAccounts.AddAsync(userAccount);
        await _context.SaveChangesAsync();
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