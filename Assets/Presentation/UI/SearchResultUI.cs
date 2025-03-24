using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FastTV.Data.Models;

namespace FastTV.Presentation
{
    public class SearchResultUI : MonoBehaviour
    {
        [SerializeField] private RawImage _posterImage;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _yearText;
        [SerializeField] private Button _button;
        [SerializeField] private LayoutElement _layoutElement;
        
        private MovieData _movie;
        private System.Action<MovieData> _onSelected;
        
        private void Awake()
        {
            _button.onClick.AddListener(OnClicked);
            
            if (_layoutElement == null)
            {
                _layoutElement = gameObject.AddComponent<LayoutElement>();
            }
            
            // Set fixed width and height for grid layout
            _layoutElement.preferredWidth = 300;
            _layoutElement.preferredHeight = 450;
        }
        
        public void Initialize(MovieData movie, System.Action<MovieData> onSelected)
        {
            _movie = movie;
            _onSelected = onSelected;
            
            _titleText.text = movie.Title;
            _yearText.text = movie.ReleaseDate?.Split('-')[0] ?? "N/A";
            
            // Set a default color while loading
            _posterImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            
            // Load poster image
            StartCoroutine(LoadPosterCoroutine());
        }
        
        private void OnClicked()
        {
            _onSelected?.Invoke(_movie);
        }
        
        private System.Collections.IEnumerator LoadPosterCoroutine()
        {
            if (string.IsNullOrEmpty(_movie.FullPosterPath))
            {
                Debug.LogWarning($"No poster path for movie: {_movie.Title}");
                yield break;
            }
            
            using (var request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(_movie.FullPosterPath))
            {
                yield return request.SendWebRequest();
                
                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    var texture = ((UnityEngine.Networking.DownloadHandlerTexture)request.downloadHandler).texture;
                    _posterImage.texture = texture;
                    _posterImage.color = Color.white;
                    
                    // Set aspect ratio
                    float aspectRatio = (float)texture.width / texture.height;
                    var rectTransform = _posterImage.GetComponent<RectTransform>();
                    var currentHeight = rectTransform.rect.height;
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentHeight * aspectRatio);
                }
                else
                {
                    Debug.LogError($"Failed to load poster for {_movie.Title}: {request.error}");
                }
            }
        }
        
        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClicked);
        }
    }
} 