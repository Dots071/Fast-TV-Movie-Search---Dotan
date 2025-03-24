using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using FastTV.Data.Models;
using FastTV.Presentation;
using Zenject;

namespace FastTV.Presentation
{
    public class UIManager : MonoBehaviour
    {
        [Header("Search View")]
        [SerializeField] private GameObject _searchView;
        [SerializeField] private TMP_InputField _searchInput;
        [SerializeField] private Button _searchButton;
        [SerializeField] private Transform _searchResultsContainer;
        [SerializeField] private GameObject _searchResultPrefab;
        [SerializeField] private GridLayoutGroup _searchResultsGrid;
        
        [Header("Pagination")]
        [SerializeField] private GameObject _paginationContainer;
        [SerializeField] private Button _prevPageButton;
        [SerializeField] private Button _nextPageButton;
        [SerializeField] private TextMeshProUGUI _pageText;
        
        [Header("Movie Details View")]
        [SerializeField] private GameObject _movieDetailsView;
        [SerializeField] private RawImage _moviePoster;
        [SerializeField] private RawImage _movieBackdrop;
        [SerializeField] private TextMeshProUGUI _movieTitle;
        [SerializeField] private TextMeshProUGUI _movieOverview;
        [SerializeField] private TextMeshProUGUI _movieRating;
        [SerializeField] private TextMeshProUGUI _movieReleaseDate;
        [SerializeField] private Button _backButton;
        
        [Header("API Key View")]
        [SerializeField] private GameObject _apiKeyView;
        [SerializeField] private TMP_InputField _apiKeyInput;
        [SerializeField] private Button _submitApiKeyButton;
        
        private SearchController _searchController;
        private MovieDetailsController _movieDetailsController;
        
        private const int MOVIES_PER_PAGE = 9;
        
        [Inject]
        public void Construct(
            SearchController searchController,
            MovieDetailsController movieDetailsController)
        {
            Debug.Log("UIManager: Construct called");
            _searchController = searchController;
            _movieDetailsController = movieDetailsController;
        }
        
        private void Start()
        {
            Debug.Log("UIManager: Start called");
            SetupEventListeners();
            SubscribeToEvents();
            ShowApiKeyView();
            Debug.Log("UIManager: Start completed");
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        
        private void SetupEventListeners()
        {
            Debug.Log("UIManager: Setting up event listeners");
            _searchButton.onClick.AddListener(OnSearchButtonClicked);
            _backButton.onClick.AddListener(OnBackButtonClicked);
            _submitApiKeyButton.onClick.AddListener(OnSubmitApiKeyClicked);
            _prevPageButton.onClick.AddListener(OnPrevPageClicked);
            _nextPageButton.onClick.AddListener(OnNextPageClicked);
        }
        
        private void SubscribeToEvents()
        {
            Debug.Log("UIManager: Subscribing to events");
            SearchController.OnSearchResultsUpdated += UpdateSearchResults;
            SearchController.OnMovieDetailsUpdated += UpdateMovieDetails;
            MovieDetailsController.OnMovieDetailsUpdated += UpdateMovieDetails;
        }
        
        private void UnsubscribeFromEvents()
        {
            SearchController.OnSearchResultsUpdated -= UpdateSearchResults;
            SearchController.OnMovieDetailsUpdated -= UpdateMovieDetails;
            MovieDetailsController.OnMovieDetailsUpdated -= UpdateMovieDetails;
        }
        
        private void OnSearchButtonClicked()
        {
            var query = _searchInput.text.Trim();
            if (!string.IsNullOrEmpty(query))
            {
                Debug.Log($"UIManager: Search requested for query: {query}");
                _searchController.HandleSearchRequest(query);
            }
        }
        
        private void OnBackButtonClicked()
        {
            ShowSearchView();
        }
        
        private void OnSubmitApiKeyClicked()
        {
            var apiKey = _apiKeyInput.text.Trim();
            if (!string.IsNullOrEmpty(apiKey))
            {
                Debug.Log("UIManager: API key submitted");
                _searchController.HandleApiKeySubmission(apiKey);
                ShowSearchView();
            }
        }
        
        private void OnPrevPageClicked()
        {
            _searchController.LoadPreviousPage();
        }
        
        private void OnNextPageClicked()
        {
            _searchController.LoadNextPage();
        }
        
        public void ShowSearchView()
        {
            Debug.Log("UIManager: Showing search view");
            _apiKeyView.SetActive(false);
            _movieDetailsView.SetActive(false);
            _searchView.SetActive(true);
            
            // Animate transition
            _searchView.transform.localScale = Vector3.zero;
            _searchView.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
        
        public void ShowMovieDetailsView()
        {
            Debug.Log("UIManager: Showing movie details view");
            _searchView.SetActive(false);
            _movieDetailsView.SetActive(true);
            
            // Animate transition
            _movieDetailsView.transform.localScale = Vector3.zero;
            _movieDetailsView.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
        
        public void ShowApiKeyView()
        {
            Debug.Log("UIManager: Showing API key view");
            _searchView.SetActive(false);
            _movieDetailsView.SetActive(false);
            _apiKeyView.SetActive(true);
            
            // Animate transition
            _apiKeyView.transform.localScale = Vector3.zero;
            _apiKeyView.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
        
        private void UpdateSearchResults(SearchResult searchResult)
        {
            Debug.Log($"UIManager: Updating search results with {searchResult.Results.Length} movies (Page {searchResult.Page} of {searchResult.TotalPages})");

            // Clear existing results
            foreach (Transform child in _searchResultsContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Hide pagination if there are no results
            if (searchResult.Results == null || searchResult.Results.Length == 0)
            {
                _paginationContainer.SetActive(false);
                return;
            }
            
            // Limit the number of movies to display per page
            var moviesToDisplay = Mathf.Min(searchResult.Results.Length, MOVIES_PER_PAGE);
            
            // Create new result items
            for (int i = 0; i < moviesToDisplay; i++)
            {
                var resultItem = Instantiate(_searchResultPrefab, _searchResultsContainer);
                var resultUI = resultItem.GetComponent<SearchResultUI>();
                resultUI.Initialize(searchResult.Results[i], OnMovieSelected);
            }

            // Update pagination controls
            UpdatePaginationControls(searchResult);
        }
        
        private void UpdatePaginationControls(SearchResult searchResult)
        {
            if (searchResult.TotalPages <= 1)
            {
                _paginationContainer.SetActive(false);
                return;
            }

            _paginationContainer.SetActive(true);
            _pageText.text = $"Page {searchResult.Page} of {searchResult.TotalPages}";
            _prevPageButton.interactable = searchResult.Page > 1;
            _nextPageButton.interactable = searchResult.Page < searchResult.TotalPages;
        }
        
        private void UpdateMovieDetails(MovieData movie)
        {
            Debug.Log($"UIManager: Updating movie details for {movie.Title}");
            _movieTitle.text = movie.Title;
            _movieOverview.text = movie.Overview;
            _movieRating.text = $"Rating: {movie.VoteAverage:F1}/10";
            _movieReleaseDate.text = $"Release Date: {movie.ReleaseDate}";
            
            // Load images
            StartCoroutine(LoadImageCoroutine(movie.FullPosterPath, _moviePoster));
            StartCoroutine(LoadImageCoroutine(movie.FullBackdropPath, _movieBackdrop));
            
            ShowMovieDetailsView();
        }
        
        private void OnMovieSelected(MovieData movie)
        {
            Debug.Log($"UIManager: Movie selected: {movie.Title}");
            _movieDetailsController.HandleMovieDetailsRequest(movie);
        }
        
        private System.Collections.IEnumerator LoadImageCoroutine(string url, RawImage targetImage)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogWarning("UIManager: Attempted to load image with empty URL");
                yield break;
            }
            
            Debug.Log($"UIManager: Loading image from {url}");
            using (var request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();
                
                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    var texture = ((UnityEngine.Networking.DownloadHandlerTexture)request.downloadHandler).texture;
                    targetImage.texture = texture;
                    targetImage.color = Color.white;
                    
                    // Set aspect ratio
                    float aspectRatio = (float)texture.width / texture.height;
                    var rectTransform = targetImage.GetComponent<RectTransform>();
                    var currentHeight = rectTransform.rect.height;
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentHeight * aspectRatio);
                    
                    Debug.Log("UIManager: Image loaded successfully");
                }
                else
                {
                    Debug.LogError($"UIManager: Failed to load image: {request.error}");
                }
            }
        }
    }
} 