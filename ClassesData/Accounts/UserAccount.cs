using System.ComponentModel.DataAnnotations.Schema;

namespace ClassesData;

public class UserAccount : Account
{
    public UserAccount(string username, string password)
        : base(username, password, AccountType.Admin)
    {
    }
}