using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.Configs;
using Gameplay.Cube;
using Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gameplay
{
    public class GameMediator : IDisposable
    {
        private readonly InputService _inputService;
        private readonly MergeSystem _mergeSystem;
        private readonly CubeConfig _cubeConfig;
        private readonly BoardConfig _boardConfig;
        private readonly ScoreSystem _scoreSystem;
        private readonly AutoMerge _autoMerge;
        private readonly CubeSpawner _cubeSpawner;

        private CubeView _currentCube;
        private bool _canLaunch = true;

        private Action _onFingerUp;
        
        public event Action<int> OnGameOver;
        public event Action<int> OnScoreChanged
        {
            add => _scoreSystem.OnScoreChanged += value;
            remove => _scoreSystem.OnScoreChanged -= value;
        }

        public GameMediator(
            CubeSpawner spawner,
            InputService inputService,
            MergeSystem mergeSystem,
            CubeConfig cubeConfig,
            BoardConfig boardConfig,
            ScoreSystem scoreSystem,
            AutoMerge autoMerge)
        {
            _cubeSpawner = spawner;
            _inputService = inputService;
            _mergeSystem = mergeSystem;
            _cubeConfig = cubeConfig;
            _boardConfig = boardConfig;
            _scoreSystem = scoreSystem;
            _autoMerge = autoMerge;
        }

        public void Construct()
        {
            SubscribeInput();
            _cubeSpawner.Initialize();
            SpawnNextCube();
        }

        public void HandleGameOver()
        {
            _canLaunch = false;
            UnsubscribeInput();
            OnGameOver?.Invoke(_scoreSystem.Score);
        }

        public void Restart()
        {
            ClearBoard();
            _scoreSystem.Reset();
            _canLaunch = true;
            SubscribeInput();
            SpawnNextCube();
        }

        public async UniTaskVoid TryExecuteAutoMergeAsync(Action onStarted, Action onFinished)
        {
            if (!_autoMerge.TryFindMergePair(out var a, out var b, _currentCube)) return;

            _canLaunch = false;
            onStarted?.Invoke();

            await _autoMerge.ExecuteAsync(a, b);

            _canLaunch = true;
            onFinished?.Invoke();
        }

        private void ClearBoard()
        {
            foreach (var cube in _mergeSystem.Cubes.ToList())
                Object.Destroy(cube.gameObject);
            
            _mergeSystem.Clear();
        }

        private void SubscribeInput()
        {
            _onFingerUp = () => LaunchCube().Forget();
            _inputService.OnFingerDrag += MoveCube;
            _inputService.OnFingerUp += _onFingerUp;
        }

        private void UnsubscribeInput()
        {
            _inputService.OnFingerDrag -= MoveCube;
            _inputService.OnFingerUp -= _onFingerUp;
        }

        private void MoveCube(float delta)
        {
            if (_currentCube == null) return;
            
            Vector3 pos = _currentCube.transform.position;
            pos.x = Mathf.Clamp(pos.x + delta * _cubeConfig.DragSensitivity, -_boardConfig.BoardHalfWidth, _boardConfig.BoardHalfWidth);
            _currentCube.transform.position = pos;
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
            _currentCube = _cubeSpawner.SpawnCube(_boardConfig.SpawnPoint);
            _mergeSystem.RegisterCube(_currentCube);
        }

        public void Dispose()
        {
            UnsubscribeInput();
            _currentCube = null;
            _canLaunch = true;
        }
    }
}