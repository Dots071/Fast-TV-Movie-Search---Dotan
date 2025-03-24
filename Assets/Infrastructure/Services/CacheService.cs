using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using FastTV.Data.Models;

namespace FastTV.Infrastructure.Services
{
    public interface ICacheService
    {
        Task SaveSearchResultsAsync(string query, MovieData[] results);
        Task<MovieData[]> GetSearchResultsAsync(string query);
        Task SaveMovieDetailsAsync(MovieData movie);
        Task<MovieData> GetMovieDetailsAsync(int movieId);
    }
    
    public class CacheService : ICacheService
    {
        private const string SEARCH_CACHE_PREFIX = "search_";
        private const string MOVIE_CACHE_PREFIX = "movie_";
        private const int CACHE_EXPIRY_DAYS = 7;
        
        public async Task SaveSearchResultsAsync(string query, MovieData[] results)
        {
            Debug.Log($"CacheService: Saving {results.Length} search results for query: {query}");
            var cacheKey = $"{SEARCH_CACHE_PREFIX}{query.ToLower()}";
            var cacheData = new CacheData<MovieData[]>
            {
                Data = results,
                Timestamp = DateTime.UtcNow
            };
            
            var json = JsonUtility.ToJson(cacheData);
            PlayerPrefs.SetString(cacheKey, json);
            PlayerPrefs.Save();
            Debug.Log($"CacheService: Saved search results with key: {cacheKey}");
            await Task.CompletedTask;
        }
        
        public async Task<MovieData[]> GetSearchResultsAsync(string query)
        {
            Debug.Log($"CacheService: Attempting to get search results for query: {query}");
            var cacheKey = $"{SEARCH_CACHE_PREFIX}{query.ToLower()}";
            var json = PlayerPrefs.GetString(cacheKey, string.Empty);
            
            if (string.IsNullOrEmpty(json))
            {
                Debug.Log($"CacheService: No cache found for key: {cacheKey}");
                return null;
            }
                
            var cacheData = JsonUtility.FromJson<CacheData<MovieData[]>>(json);
            if (IsCacheExpired(cacheData.Timestamp))
            {
                Debug.Log($"CacheService: Cache expired for key: {cacheKey}");
                PlayerPrefs.DeleteKey(cacheKey);
                PlayerPrefs.Save();
                return null;
            }
            
            Debug.Log($"CacheService: Found valid cache for key: {cacheKey} with {cacheData.Data.Length} results");
            await Task.CompletedTask;
            return cacheData.Data;
        }
        
        public async Task SaveMovieDetailsAsync(MovieData movie)
        {
            Debug.Log($"CacheService: Saving details for movie: {movie.Title}");
            var cacheKey = $"{MOVIE_CACHE_PREFIX}{movie.Id}";
            var cacheData = new CacheData<MovieData>
            {
                Data = movie,
                Timestamp = DateTime.UtcNow
            };
            
            var json = JsonUtility.ToJson(cacheData);
            PlayerPrefs.SetString(cacheKey, json);
            PlayerPrefs.Save();
            Debug.Log($"CacheService: Saved movie details with key: {cacheKey}");
            await Task.CompletedTask;
        }
        
        public async Task<MovieData> GetMovieDetailsAsync(int movieId)
        {
            Debug.Log($"CacheService: Attempting to get details for movie ID: {movieId}");
            var cacheKey = $"{MOVIE_CACHE_PREFIX}{movieId}";
            var json = PlayerPrefs.GetString(cacheKey, string.Empty);
            
            if (string.IsNullOrEmpty(json))
            {
                Debug.Log($"CacheService: No cache found for key: {cacheKey}");
                return null;
            }
                
            var cacheData = JsonUtility.FromJson<CacheData<MovieData>>(json);
            if (IsCacheExpired(cacheData.Timestamp))
            {
                Debug.Log($"CacheService: Cache expired for key: {cacheKey}");
                PlayerPrefs.DeleteKey(cacheKey);
                PlayerPrefs.Save();
                return null;
            }
            
            Debug.Log($"CacheService: Found valid cache for key: {cacheKey} for movie: {cacheData.Data.Title}");
            await Task.CompletedTask;
            return cacheData.Data;
        }
        
        private bool IsCacheExpired(DateTime timestamp)
        {
            var expired = (DateTime.UtcNow - timestamp).TotalDays > CACHE_EXPIRY_DAYS;
            if (expired)
            {
                Debug.Log($"CacheService: Cache from {timestamp} has expired");
            }
            return expired;
        }
        
        [Serializable]
        private class CacheData<T>
        {
            public T Data;
            public DateTime Timestamp;
        }
    }
} 