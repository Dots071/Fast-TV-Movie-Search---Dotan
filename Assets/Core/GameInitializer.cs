using UnityEngine;
using Zenject;
using FastTV.Infrastructure.Services;
using FastTV.Presentation;

namespace FastTV.Core
{
    public class GameInitializer : IInitializable
    {
        private readonly TMDbService _tmdbService;
        private readonly SearchController _searchController;
        private readonly MovieDetailsController _movieDetailsController;

        public GameInitializer(
            TMDbService tmdbService,
            SearchController searchController,
            MovieDetailsController movieDetailsController)
        {
            _tmdbService = tmdbService;
            _searchController = searchController;
            _movieDetailsController = movieDetailsController;
        }

        public void Initialize()
        {
            Debug.Log("GameInitializer: Starting initialization");
            
            Debug.Log("GameInitializer: Initializing TMDbService");
            _tmdbService.Initialize();
            
            Debug.Log("GameInitializer: Initializing SearchController");
            _searchController.Initialize();
            
            Debug.Log("GameInitializer: Initializing MovieDetailsController");
            _movieDetailsController.Initialize();
            
            Debug.Log("GameInitializer: Initialization completed");
        }
    }
} 