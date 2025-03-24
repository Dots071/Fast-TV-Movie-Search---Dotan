using UnityEngine;
using Zenject;
using FastTV.Infrastructure.Services;
using FastTV.Data;
using FastTV.Presentation;

namespace FastTV.Core
{
    [CreateAssetMenu(fileName = "GameInstaller", menuName = "FastTV/Installers/GameInstaller")]
    public class GameInstaller : ScriptableObjectInstaller<GameInstaller>
    {
        [SerializeField] private GameObject _uiCanvasPrefab;

        public override void InstallBindings()
        {
            // Core
            Container.BindInterfacesAndSelfTo<GameBootstrap>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameInitializer>().AsSingle();
            
            // Infrastructure
            Container.BindInterfacesAndSelfTo<TMDbService>().AsSingle();
            Container.BindInterfacesAndSelfTo<CacheService>().AsSingle();
            
            // Data
            Container.BindInterfacesAndSelfTo<MovieRepository>().AsSingle();
            
            // Presentation
            Container.Bind<UIManager>()
                .FromComponentInNewPrefab(_uiCanvasPrefab)
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<SearchController>().AsSingle();
            Container.BindInterfacesAndSelfTo<MovieDetailsController>().AsSingle();
        }
    }
} 