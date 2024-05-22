using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassesData.Music;

[Table("music_files")]
public class MusicFile
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string FileName { get; set; }

    [Required]
    [StringLength(255)]
    public string FilePath { get; set; }

    [Required]
    [StringLength(255)]
    public string Authors { get; set; } 

    [Required]
    public TimeSpan Duration { get; set; } 

    public int Auditions { get; set; } 


}