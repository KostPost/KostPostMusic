using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassesData.Music
{
    [Table("playlists")]
    public class Playlist
    {
        [Column("id")] [Key] public int Id { get; set; }

        [Required] [StringLength(255)] [Column("name")] public string Name { get; set; }

        [Required] [Column("owner_id")] public int OwnerId { get; set; }

        [Column("description")] public string Description { get; set; }

        public virtual ICollection<MusicData> Songs { get; set; } = new List<MusicData>();
    }
}