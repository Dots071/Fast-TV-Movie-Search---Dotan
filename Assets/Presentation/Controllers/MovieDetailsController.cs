using System;
using System.Threading.Tasks;
using FastTV.Data;
using FastTV.Data.Models;
using FastTV.Infrastructure.Services;
using Zenject;
using UnityEngine;

namespace FastTV.Presentation
{
    public class MovieDetailsController : IInitializable
    {
        private readonly IMovieRepository _movieRepository;
        
        public static event Action<MovieData> OnMovieDetailsRequested;
        public static event Action<MovieData> OnMovieDetailsUpdated;
        
        public MovieDetailsController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }
        
        public void Initialize()
        {
            // No need to subscribe to events anymore
        }
        
        public async void HandleMovieDetailsRequest(MovieData movie)
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