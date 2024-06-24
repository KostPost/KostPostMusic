using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;


namespace ClassesData.Music
{
    [Table("playlists")]
    public class Playlist
    {
        [JsonPropertyName("id")] [Key] public int Id { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

      //   [JsonPropertyName("description")] public string Description { get; set; }

        [JsonPropertyName("songIds")] public List<int> SongIds { get; set; } = new List<int>();

        // [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; } = DateTime.Now;
        //
        // [JsonPropertyName("updatedAt")] public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonPropertyName("createdBy")] public int CreatedBy { get; set; }
    }
}