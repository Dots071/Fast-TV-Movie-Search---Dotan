using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

namespace FastTV.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [Inject] private GameInitializer _gameInitializer;
        
        private async void Start()
        {
            Debug.Log("GameBootstrap: Start called");
            await InitializeGame();
        }

        private async UniTask InitializeGame()
        {
            try
            {
                Debug.Log("GameBootstrap: Starting game initialization");
                _gameInitializer.Initialize();
                Debug.Log("GameBootstrap: Game initialization completed");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to initialize game: {e}");
            }
        }
    }
} 