using System.Threading.Tasks;
using FastTV.Data.Models;
using FastTV.Infrastructure.Services;
using UnityEngine;

namespace FastTV.Data
{
    public interface IMovieRepository
    {
        Task<SearchResult> SearchMoviesAsync(string query, int page = 1);
        Task<MovieData> GetMovieDetailsAsync(int movieId);
    }
    
    public class MovieRepository : IMovieRepository
    {
        private readonly ITMDbService _tmdbService;
        private readonly ICacheService _cacheService;
        
        public MovieRepository(ITMDbService tmdbService, ICacheService cacheService)
        {
            _tmdbService = tmdbService;
            _cacheService = cacheService;
            Debug.Log("MovieRepository: Initialized");
        }
        
        public async Task<SearchResult> SearchMoviesAsync(string query, int page = 1)
        {
            Debug.Log($"MovieRepository: Searching for movies with query: {query}, page: {page}");
            
            // Try to get from cache first
            var cachedResults = await _cacheService.GetSearchResultsAsync($"{query}_page{page}");
            if (cachedResults != null)
            {
                Debug.Log($"MovieRepository: Found {cachedResults.Length} cached results for query: {query}, page: {page}");
                return new SearchResult { Results = cachedResults, Page = page };
            }
            
            Debug.Log($"MovieRepository: No cache found for query: {query}, page: {page}, fetching from API");
            // If not in cache, fetch from API
            var searchResult = await _tmdbService.SearchMoviesAsync(query, page);
            
            Debug.Log($"MovieRepository: Caching {searchResult.Results.Length} results for query: {query}, page: {page}");
            // Cache the results
            await _cacheService.SaveSearchResultsAsync($"{query}_page{page}", searchResult.Results);
            
            return searchResult;
        }
        
        public async Task<MovieData> GetMovieDetailsAsync(int movieId)
        {
            Debug.Log($"MovieRepository: Getting details for movie ID: {movieId}");
            
            // Try to get from cache first
            var cachedMovie = await _cacheService.GetMovieDetailsAsync(movieId);
            if (cachedMovie != null)
            {
                Debug.Log($"MovieRepository: Found cached details for movie: {cachedMovie.Title}");
                return cachedMovie;
            }
            
            Debug.Log($"MovieRepository: No cache found for movie ID: {movieId}, fetching from API");
            // If not in cache, fetch from API
            var movie = await _tmdbService.GetMovieDetailsAsync(movieId);
            
            Debug.Log($"MovieRepository: Caching details for movie: {movie.Title}");
            // Cache the movie details
            await _cacheService.SaveMovieDetailsAsync(movie);
            
            return movie;
        }
    }
} 