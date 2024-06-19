using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassesData.Music
{
    [Table("albums")]
    public class Album
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public virtual ICollection<MusicFile> MusicFiles { get; set; }
    }

    [Table("music_files")]
    public class MusicFile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [Required]
        public int AlbumId { get; set; }

        [ForeignKey("AlbumId")]
        public virtual Album Album { get; set; }

        public virtual ICollection<MusicAuthor> Authors { get; set; }
    }

    [Table("music_authors")]
    public class MusicAuthor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public virtual ICollection<MusicFile> MusicFiles { get; set; }
    }
}