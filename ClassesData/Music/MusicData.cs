﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassesData.Music
{
    [Table("music_data")]
    public class MusicData
    {
        [Column("id")] [Key] public int Id { get; set; }

        [Required] [StringLength(255)] [Column("file_name")] public string FileName { get; set; }

        [Required] [Column("author_id")] public int AuthorID { get; set; }

        [Required] [StringLength(255)] [Column("author_name")] public string AuthorName { get; set; }

        [Column("play_count")] public int PlayCount { get; set; } = 0;

        [Column("duration")] public TimeSpan Duration { get; set; }
        
        [NotMapped]
        public string FormattedDuration => Duration.ToString(@"mm\:ss");
    }
}