using System;
using System.Threading.Tasks;
using FastTV.Data;
using FastTV.Data.Models;
using FastTV.Infrastructure.Services;
using Zenject;
using UnityEngine;

namespace FastTV.Presentation
{
    public class SearchController : IInitializable
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ITMDbService _tmdbService;
        
        public static event Action<SearchResult> OnSearchResultsUpdated;
        public static event Action<string> OnApiKeySubmitted;
        public static event Action<MovieData> OnMovieSelected;
        public static event Action<MovieData> OnMovieDetailsUpdated;
        
        private int _currentPage = 1;
        private string _currentQuery = string.Empty;
        private bool _isLoading = false;
        
        public SearchController(
            IMovieRepository movieRepository,
            ITMDbService tmdbService)
        {
            _movieRepository = movieRepository;
            _tmdbService = tmdbService;
        }
        
        public void Initialize()
        {
            // No initialization needed
        }
        
        public async void HandleSearchRequest(string query, bool newSearch = true)
        {
            if (_isLoading) return;
            
            try
            {
                _isLoading = true;
                
                if (newSearch)
                {
                    _currentPage = 1;
                    _currentQuery = query;
                }
                
                Debug.Log($"SearchController: Searching for '{_currentQuery}' on page {_currentPage}");
                var searchResult = await _movieRepository.SearchMoviesAsync(_currentQuery, _currentPage);
                OnSearchResultsUpdated?.Invoke(searchResult);
                
                Debug.Log($"SearchController: Found {searchResult.Results.Length} results. Page {searchResult.Page} of {searchResult.TotalPages}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to search movies: {e}");
                // TODO: Show error message to user
            }
            finally
            {
                _isLoading = false;
            }
        }
        
        public void LoadNextPage()
        {
            if (!string.IsNullOrEmpty(_currentQuery) && !_isLoading)
            {
                _currentPage++;
                HandleSearchRequest(_currentQuery, false);
            }
        }
        
        public void LoadPreviousPage()
        {
            if (!string.IsNullOrEmpty(_currentQuery) && !_isLoading && _currentPage > 1)
            {
                _currentPage--;
                HandleSearchRequest(_currentQuery, false);
            }
        }
        
        public void HandleApiKeySubmission(string apiKey)
        {
            try
            {
                _tmdbService.SetApiKey(apiKey);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to set API key: {e}");
                // TODO: Show error message to user
            }
        }
        
        public async void HandleMovieSelection(MovieData movie)
        {
            try
            {
                var detailedMovie = await _movieRepository.GetMovieDetailsAsync(movie.Id);
                OnMovieDetailsUpdated?.Invoke(detailedMovie);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to get movie details: {e}");
                // TODO: Show error message to user
            }
        }
    }
} 