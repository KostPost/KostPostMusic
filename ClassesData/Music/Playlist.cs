﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ClassesData.Music
{
    [Table("playlists")]
    public class Playlist
    {
        [JsonPropertyName("id")] [Key] public int Id { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("songIds")] public List<int> SongIds { get; set; } = new List<int>();

        [JsonPropertyName("createdBy")] public int CreatedBy { get; set; }

        [JsonPropertyName("songAddedTimes")]
        public Dictionary<int, DateTime> SongAddedTimes { get; set; } = new Dictionary<int, DateTime>();
    }
}