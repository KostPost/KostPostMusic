using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassesData;

[Table("accounts")]
public abstract class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; }

    [Required]
    [StringLength(100)]
    public string Password { get; set; }

    [Required]
    public AccountType AccountType { get; set; }
    
    public Account(string username, string password, AccountType accountType)
    {
        Username = username;
        Password = password;
        AccountType = accountType;
    }
}

public enum AccountType
{
    User,
    Admin
}