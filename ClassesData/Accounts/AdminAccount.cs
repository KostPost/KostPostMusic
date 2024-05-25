using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassesData;

public class AdminAccount : Account
{
    [Required] public int AdminLevel { get; set; }


    public AdminAccount(string username, string password, int adminLevel)
        : base(username, password, AccountType.Admin)
    {
        AdminLevel = adminLevel;
    }
}