# Fast TV Movie Explorer

A Unity-based movie exploration application that interfaces with The Movie Database (TMDb) API. This application allows users to search for movies, view detailed information, and explore movie metadata in a responsive and user-friendly interface.

## Features

- Movie search functionality with real-time results
- Detailed movie information view
- High-quality movie posters and backdrops
- Responsive UI for both portrait and landscape orientations
- Secure API key management
- Local caching for improved performance
- Pagination support for search results
- Fast and smooth transitions between views

## Architecture Overview

The application follows the MVCS (Model-View-Controller-Service) architecture pattern with dependency injection using Zenject:

### Core Components

- **Models**: Data structures representing movie information (`MovieData`, `SearchResult`)
- **Views**: UI components for displaying data (`UIManager`, `SearchResultUI`)
- **Controllers**: Business logic handlers (`SearchController`, `MovieDetailsController`)
- **Services**: External integrations and utilities (`TMDbService`, `CacheService`)

### Design Patterns Used

- **Dependency Injection**: Using Zenject for loose coupling and better testability
- **Repository Pattern**: Abstracting data access through `MovieRepository`
- **Observer Pattern**: Using events for communication between components
- **Factory Pattern**: For creating UI elements and managing object lifecycle
- **Command Pattern**: For handling user interactions and API requests

## Technical Implementation

### API Integration
- Integration with TMDb API v3
- Proper error handling and logging
- Response caching with expiration
- Asynchronous operations using UniTask

### UI/UX Features
- Grid layout for search results (3x3)
- Smooth transitions using DOTween
- Loading states and error handling
- Responsive design for different screen sizes
- Portrait and landscape support

### Caching System
- Local caching of search results and movie details
- Cache expiration management
- Memory-efficient data storage

## Setup Instructions

1. Clone the repository
```bash
git clone https://github.com/Dots071/fast-tv-movie-explorer.git
```

2. Open the project in Unity 2022.3 or later

3. Install required packages:
   - DOTween
   - Zenject
   - UniTask
   - Newtonsoft.Json

4. Get a TMDb API key:
   - Visit [TMDb Developer Portal](https://www.themoviedb.org/settings/api)
   - Create an account and request an API key
   - Enter the API key when first launching the app

## Building the Project

### Development Build
1. Open the project in Unity
2. Set the build target to Android
3. Configure the following Player Settings:
   - Minimum API Level: Android 5.0 (API level 21)
   - Target API Level: Android 13.0 (API level 33)
   - Package Name: com.fasttv.movieexplorer
4. Build and Run

### Release Build
The project includes a GitHub Actions workflow that automatically:
- Builds the project
- Runs unit tests
- Generates an APK
- Creates a GitHub release with the APK attached

## Known Limitations and Future Improvements

### Current Limitations
- Limited offline functionality
- Basic error messaging
- No advanced search filters
- Limited movie metadata display

### Planned Improvements
- Enhanced offline mode with local database
- Advanced search filters (year, rating, genre)
- Cast and crew information
- Movie recommendations
- Favorite movies functionality
- Enhanced error handling and user feedback
- Unit test coverage expansion

## Dependencies

- Unity 2022.3+
- DOTween
- Zenject
- UniTask
- Newtonsoft.Json
- Android SDK 21+

## License

This project is part of a technical assessment and is not licensed for public use.

## Acknowledgments

- [TMDb](https://www.themoviedb.org/) for providing the movie database API
- The Unity community for various helpful resources and packages