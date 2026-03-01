using System;
using Gameplay;
using Services;

namespace UI
{
    public class UIMediator : IDisposable
    {
        private readonly GenericFactory _genericFactory;
        private readonly GameMediator _gameMediator;

        private ScoreView _scoreView;
        private DeadLineView _deadLine;
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
            _scoreView = _genericFactory.Create<ScoreView>(Constants.ScoreViewPath);

            _deadLine = _genericFactory.Create<DeadLineView>(Constants.DeadLinePath);

            _gameOverView = _genericFactory.Create<GameOverView>(Constants.GameOverViewPath);
            _gameOverView.RestartButton.onClick.AddListener(OnRestartClicked);
            _gameOverView.Hide();

            _autoMergeButton = _genericFactory.Create<AutoMergeButtonView>(Constants.AutoMergeButtonPath);
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
            _deadLine.OnGameOver += _gameMediator.HandleGameOver;
            _gameMediator.OnGameOver += HandleGameOver;
        }

        private void Unsubscribe()
        {
            _gameMediator.OnGameOver -= HandleGameOver;
            _gameMediator.OnScoreChanged -= _scoreView.UpdateScore;
            _gameMediator.OnGameOver -= HandleGameOver;
        }

        public void Dispose()
        {
            Unsubscribe();
            _gameOverView.RestartButton.onClick.RemoveListener(OnRestartClicked);
        }
    }
}