using System;
using Gameplay;
using Services;
using UnityEngine;

namespace UI
{
    public class UIMediator : IDisposable
    {
        private readonly GenericFactory _genericFactory;
        private readonly GameMediator _gameMediator;

        private Canvas _mainCanvas;
        private ScoreView _scoreView;
        private GameOverView _gameOverView;
        private AutoMergeButtonView _autoMergeButton;

        public UIMediator(
            GenericFactory genericFactory,
            GameMediator gameMediator)
        {
            _genericFactory = genericFactory;
            _gameMediator = gameMediator;
        }

        public void Construct()
        {
            _mainCanvas = _genericFactory.Create<Canvas>(Constants.MainCanvasView);
            
            _scoreView = _genericFactory.Create<ScoreView>(Constants.ScoreViewPath, _mainCanvas.transform);

            _gameOverView = _genericFactory.Create<GameOverView>(Constants.GameOverViewPath, _mainCanvas.transform);
            _gameOverView.RestartButton.onClick.AddListener(OnRestartClicked);
            _gameOverView.Hide();

            _autoMergeButton = _genericFactory.Create<AutoMergeButtonView>(Constants.AutoMergeButtonPath, _mainCanvas.transform);
            _autoMergeButton.OnClick(() =>
                _gameMediator.TryExecuteAutoMergeAsync(
                    onStarted: () => _autoMergeButton.SetInteractable(false),
                    onFinished: () => _autoMergeButton.SetInteractable(true)
                ).Forget()
            );

            Subscribe();
        }

        private void HandleGameOver(int score)
        {
            _gameOverView.Show(score);
        }

        private void OnRestartClicked()
        {
            _gameOverView.Hide();
            _gameMediator.Restart();
        }
        
        private void Subscribe()
        {
            _gameMediator.OnScoreChanged += _scoreView.UpdateScore;
            _gameMediator.OnGameOver += HandleGameOver;
        }

        private void Unsubscribe()
        {
            _gameMediator.OnGameOver -= HandleGameOver;
            _gameMediator.OnScoreChanged -= _scoreView.UpdateScore;
        }

        public void Dispose()
        {
            Unsubscribe();
            _gameOverView.RestartButton.onClick.RemoveListener(OnRestartClicked);
        }
    }
}