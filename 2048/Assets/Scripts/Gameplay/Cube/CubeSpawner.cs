using Gameplay.Configs;
using Services;
using UnityEngine;
using Zenject;

namespace Gameplay.Cube
{
    public class CubeSpawner
    {
        private readonly IAssetProviderService _assetProvider;
        private readonly DiContainer _container;

        private CubeView _cubePrefab;
        private CubeConfig _cubeConfig;

        public CubeSpawner(IAssetProviderService assetProvider, DiContainer container)
        {
            _assetProvider = assetProvider;
            _container = container;
        }

        public void Initialize()
        {
            _cubePrefab = _assetProvider.LoadAssetFromResources<CubeView>(Constants.CubeViewPath);
            _cubeConfig = _assetProvider.LoadAssetFromResources<CubeConfig>(Constants.CubeConfigPath);
        }

        public CubeView SpawnCube(Vector3 position)
        {
            int value = Random.value < 0.75f ? 2 : 4;
            CubeView cube = _container.InstantiatePrefabForComponent<CubeView>(_cubePrefab, position, Quaternion.identity, null);
            cube.Init(value, _cubeConfig);
            return cube;
        }
    }
}