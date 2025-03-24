using System;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

namespace FastTV.Data.Models
{
    [Serializable]
    public class MovieData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("vote_average")]
        public float VoteAverage { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }
        
        [JsonProperty("genres")]
        private Genre[] _genres;
        
        [JsonIgnore]
        public string[] Genres => _genres?.Select(g => g.Name).ToArray() ?? new string[0];
        
        [JsonIgnore]
        public string FullPosterPath => !string.IsNullOrEmpty(PosterPath) 
            ? $"https://image.tmdb.org/t/p/w500{PosterPath}" 
            : string.Empty;
            
        [JsonIgnore]
        public string FullBackdropPath => !string.IsNullOrEmpty(BackdropPath) 
            ? $"https://image.tmdb.org/t/p/original{BackdropPath}" 
            : string.Empty;
            
        [Serializable]
        private class Genre
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }
    }
} 