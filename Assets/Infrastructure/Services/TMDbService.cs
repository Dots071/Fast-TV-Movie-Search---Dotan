using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using FastTV.Data.Models;
using Zenject;

namespace FastTV.Infrastructure.Services
{
    public class TMDbService : ITMDbService, IInitializable
    {
        private const string BASE_URL = "https://api.themoviedb.org/3";
        private string _apiKey;
        
        public void Initialize()
        {
            _apiKey = PlayerPrefs.GetString("TMDbApiKey", string.Empty);
            Debug.Log($"TMDbService: Initialized with API key present: {!string.IsNullOrEmpty(_apiKey)}");
        }
        
        public void SetApiKey(string apiKey)
        {
            Debug.Log($"TMDbService: Setting new API key: {apiKey}");
            _apiKey = apiKey;
            PlayerPrefs.SetString("TMDbApiKey", apiKey);
        }
        
        public bool HasValidApiKey()
        {
            return !string.IsNullOrEmpty(_apiKey);
        }
        
        public async Task<SearchResult> SearchMoviesAsync(string query, int page = 1)
        {
            Debug.Log($"TMDbService: Searching movies with query: {query}, page: {page}");
            
            if (!HasValidApiKey())
            {
                throw new InvalidOperationException("TMDb API key not set");
            }
            
            var url = $"{BASE_URL}/search/movie?api_key={_apiKey}&query={UnityWebRequest.EscapeURL(query)}&page={page}";
            Debug.Log($"TMDbService: Making request to: {url.Replace(_apiKey, "API_KEY")}");
            
            var response = await SendRequestAsync(url);
            Debug.Log($"TMDbService: Raw search response: {response}");
            
            var searchResult = JsonConvert.DeserializeObject<SearchResult>(response);
            Debug.Log($"TMDbService: Deserialized {searchResult?.Results?.Length ?? 0} results. Total pages: {searchResult?.TotalPages}, Total results: {searchResult?.TotalResults}");
            
            if (searchResult?.Results != null)
            {
                foreach (var movie in searchResult.Results)
                {
                    Debug.Log($"TMDbService: Movie: {movie.Title}, ID: {movie.Id}, PosterPath: {movie.PosterPath}, FullPosterPath: {movie.FullPosterPath}");
                }
            }
            
            return searchResult ?? new SearchResult { Results = Array.Empty<MovieData>() };
        }
        
        public async Task<MovieData> GetMovieDetailsAsync(int movieId)
        {
            Debug.Log($"TMDbService: Getting details for movie ID: {movieId}");
            
            if (!HasValidApiKey())
            {
                throw new InvalidOperationException("TMDb API key not set");
            }
            
            var url = $"{BASE_URL}/movie/{movieId}?api_key={_apiKey}";
            Debug.Log($"TMDbService: Making request to: {url.Replace(_apiKey, "API_KEY")}");
            
            var response = await SendRequestAsync(url);
            Debug.Log($"TMDbService: Raw movie details response: {response}");
            
            var movie = JsonConvert.DeserializeObject<MovieData>(response);
            Debug.Log($"TMDbService: Deserialized movie details - Title: {movie?.Title}, ID: {movie?.Id}, PosterPath: {movie?.PosterPath}, FullPosterPath: {movie?.FullPosterPath}");
            
            return movie;
        }
        
        private async Task<string> SendRequestAsync(string url)
        {
            using (var request = UnityWebRequest.Get(url))
            {
                Debug.Log($"TMDbService: Starting request to: {url.Replace(_apiKey, "API_KEY")}");
                var operation = request.SendWebRequest();
                
                while (!operation.isDone)
                    await Task.Yield();
                
                if (request.result != UnityWebRequest.Result.Success)
                {
                    var error = $"TMDb API request failed: {request.error}";
                    Debug.LogError(error);
                    throw new Exception(error);
                }
                
                Debug.Log("TMDbService: Request completed successfully");
                return request.downloadHandler.text;
            }
        }
    }
} 