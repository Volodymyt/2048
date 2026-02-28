using System;
using Cysharp.Threading.Tasks;
using Gameplay.Cube;
using Services;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class GameMediator : IDisposable
    {
        private readonly InputService _inputService;
        private readonly MergeService _mergeService;
        private readonly CubeConfig _cubeConfig;
        private readonly GenericFactory _genericFactory;
        private readonly ScoreService _scoreService;
        
        private CubeSpawner _cubeSpawner;
        private CubeView _currentCube;
        private ScoreView _scoreView;

        private float _dragSensitivity = 0.01f;
        private float _boardHalfWidth = 2.478f;

        private Vector3 _spawnPoint = new Vector3(0,-2.288f,-22.27f);

        public GameMediator(
            CubeSpawner spawner,
            InputService inputService,
            MergeService mergeService,
            CubeConfig cubeConfig,
            GenericFactory genericFactory,
            ScoreService scoreService)
        {
            _cubeSpawner = spawner;
            _inputService = inputService;
            _mergeService = mergeService;
            _cubeConfig = cubeConfig;
            _genericFactory = genericFactory;
            _scoreService = scoreService;
        }
        
        public void Construct()
        {
            _inputService.OnFingerDrag += MoveCube;
            _inputService.OnFingerUp += () => LaunchCube().Forget();
            _cubeSpawner.Initialize();
            SpawnNextCube();
            
            _scoreView = _genericFactory.Create<ScoreView>(Constants.ScoreViewPath);
            _scoreService.OnScoreChanged += _scoreView.UpdateScore;
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
    
            _currentCube.Launch(Vector3.forward * _cubeConfig.LaunchForce);
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
            _scoreService.OnScoreChanged -= _scoreView.UpdateScore;
        }
    }
}