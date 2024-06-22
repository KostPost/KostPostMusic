using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassesData;

[Table("accounts")]
public class Account
{
    [Key] [Column("id")] public int Id { get; set; }

    [Required] [StringLength(100)] [Column("username")]  public string Username { get; set; }

    [Required] [StringLength(100)] [Column("password")]  public string Password { get; set; }

    [Required] [StringLength(100)] [Column("account_type")] public string AccountType { get; set; }

    [Column("admin_level")] public int? AdminLevel { get; set; }


    public Account(string username, string password,string accountType)
    {
        Username = username;
        Password = password;
        AccountType = accountType;

        if (accountType == ClassesData.AccountType.User.ToString())
        {
            AdminLevel = null;
        }
    }
    public Account(int id,string username, string password,string accountType)
    {
        Id = id;
        Username = username;
        Password = password;
        AccountType = accountType;

        if (accountType == ClassesData.AccountType.User.ToString())
        {
            AdminLevel = null;
        }
    }
    
}

public enum AccountType
{
    User,
    Admin
}