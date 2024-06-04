using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassesData.Music

[Table("music_files")]
public class MusicFile
{
    [Key] public int Id { get; set; }

    [Required] [StringLength(255)] public string FileName { get; set; }

    [Required] [StringLength(255)] public string Album { get; set; }

    public virtual ICollection<MusicAuthor> Authors { get; set; }
}

[Table("music_authors")]
public class MusicAuthor
{
    [Key] public int Id { get; set; }

    [Required] [StringLength(255)] public string Name { get; set; }

    public virtual ICollection<MusicFile> MusicFiles { get; set; }
}