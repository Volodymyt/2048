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
        private readonly AutoMergeService _autoMergeService;

        private CubeSpawner _cubeSpawner;
        private CubeView _currentCube;
        private ScoreView _scoreView;
        private GameOverTriggerView _deadLine;
        private GameOverView _gameOverView;
        private AutoMergeButtonView _autoMergeButton;

        private float _dragSensitivity = 0.01f;
        private float _boardHalfWidth = 2.478f;

        private Vector3 _spawnPoint = new Vector3(0,-2.288f,-22.27f);

        public GameMediator(
            CubeSpawner spawner,
            InputService inputService,
            MergeService mergeService,
            CubeConfig cubeConfig,
            GenericFactory genericFactory,
            ScoreService scoreService,
            AutoMergeService autoMergeService)
        {
            _cubeSpawner = spawner;
            _inputService = inputService;
            _mergeService = mergeService;
            _cubeConfig = cubeConfig;
            _genericFactory = genericFactory;
            _scoreService = scoreService;
            _autoMergeService = autoMergeService;
        }
        
        public void Construct()
        {
            _inputService.OnFingerDrag += MoveCube;
            _inputService.OnFingerUp += () => LaunchCube().Forget();
            _cubeSpawner.Initialize();
            SpawnNextCube();
            
            _scoreView = _genericFactory.Create<ScoreView>(Constants.ScoreViewPath);
            _scoreService.OnScoreChanged += _scoreView.UpdateScore;
            
            _deadLine = _genericFactory.Create<GameOverTriggerView>(Constants.DeadLinePath);
            _deadLine.OnGameOver += HandleGameOver;
            _gameOverView = _genericFactory.Create<GameOverView>(Constants.GameOverViewPath);
            _gameOverView.RestartButton.onClick.AddListener(Restart);
            _gameOverView.Hide();
            
            _autoMergeButton = _genericFactory.Create<AutoMergeButtonView>(Constants.AutoMergeButtonPath);
            _autoMergeButton.OnClick(() => OnAutoMergeClicked().Forget());
        }

        private async UniTaskVoid OnAutoMergeClicked()
        {
            if (!_autoMergeService.TryFindMergePair(out var a, out var b, _currentCube)) return;

            _autoMergeButton.SetInteractable(false);
            _canLaunch = false;

            await _autoMergeService.ExecuteAsync(a, b);

            _canLaunch = true;
            _autoMergeButton.SetInteractable(true);
        }
        
        private void Restart()
        {
            _scoreService.Reset();
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

        private void HandleGameOver()
        {
            _canLaunch = false;
            _inputService.OnFingerDrag -= MoveCube;
            _gameOverView.Show(_scoreService.Score);

        }
        
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
            _deadLine.OnGameOver -= HandleGameOver;
            _scoreService.OnScoreChanged -= _scoreView.UpdateScore;
        }
    }
}