using System;
using Cubes.Merge;
using Cysharp.Threading.Tasks;
using Gameplay.Cube;
using Services;
using UnityEngine;

namespace Gameplay
{
    public class GameMediator : IDisposable
    {
        private readonly InputService _inputService;
        private readonly MergeService _mergeService;
        
        private CubeSpawner _cubeSpawner;
        private CubeView _currentCube;

        private float _dragSensitivity = 0.01f;
        private float _boardHalfWidth = 2.478f;
        private float _launchForce = 30;

        private Vector3 _spawnPoint = new Vector3(0,-2.288f,-22.27f);

        public GameMediator(
            CubeSpawner spawner,
            InputService inputService,
            MergeService mergeService)
        {
            _cubeSpawner = spawner;
            _inputService = inputService;
            _mergeService = mergeService;
        }
        
        public void Construct()
        {
            _inputService.OnFingerDrag += MoveCube;
            _inputService.OnFingerUp += () => LaunchCube().Forget();
            _cubeSpawner.Initialize();
            SpawnNextCube();
        }

        private void MoveCube(float delta)
        {
            if (_currentCube == null) return;
            Vector3 pos = _currentCube.transform.position;
            pos.x += delta * _dragSensitivity;
            pos.x = Mathf.Clamp(pos.x, -_boardHalfWidth, _boardHalfWidth);
            _currentCube.transform.position = pos;
        }

        private bool _canLaunch = true;

        private async UniTaskVoid LaunchCube()
        {
            if (_currentCube == null || !_canLaunch) return;
            _canLaunch = false;
    
            _currentCube.Launch(Vector3.forward * _launchForce);
            _currentCube = null;
    
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
    
            SpawnNextCube();
            _canLaunch = true;
        }
        
        private void SpawnNextCube()
        {
            _currentCube = _cubeSpawner.SpawnCube(_spawnPoint);
            _mergeService.RegisterCube(_currentCube);
        }

        public void Dispose()
        {
            
        }
    }
}