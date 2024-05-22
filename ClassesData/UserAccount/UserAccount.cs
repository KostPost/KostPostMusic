using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ClassesData;

[Table("user_account")]
public class UserAccount
{
    [Key] [Column("user_id")] public long Id { get; set; }

    [Column("username")] public String Username { get; set; }

    [Column("password")] public String Password { get; set; }

    [Column("account_type")] public AccountType AccountType { get; set; }

    public UserAccount()
    {
    }

    public UserAccount(string username, string password)
    {
        if (username == null)
            throw new ArgumentNullException(nameof(username), "Username cannot be null");

        if (password == null)
            throw new ArgumentNullException(nameof(password), "Password cannot be null");

        Username = username;
        Password = password;
    }
}

public enum AccountType
{
    User,
    Admin
}