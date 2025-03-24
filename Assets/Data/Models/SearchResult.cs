using System;
using Newtonsoft.Json;
using FastTV.Data.Models;

namespace FastTV.Data.Models
{
    [Serializable]
    public class SearchResult
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("results")]
        public MovieData[] Results { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("total_results")]
        public int TotalResults { get; set; }
    }
} 