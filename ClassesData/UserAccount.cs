using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseActions;


[Table("user_account")]
public class UserAccount
{
    [Key]
    [Column("user_id")]
    public long Id { get; set; }
    
    [Column("username")]
    public String Username { get; set; }
    
    [Column("password")]
    public String Password { get; set; }
}