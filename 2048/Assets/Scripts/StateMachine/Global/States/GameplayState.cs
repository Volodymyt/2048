using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using StateMachine.Base;
using UI;

namespace StateMachine.Global.States
{
    public class GameplayState : State
    {
        private const string SceneName = "Game Scene";

        private readonly GameMediator _gameMediator;
        private readonly UIMediator _uiMediator;

        public GameplayState(GameMediator gameMediator, UIMediator uiMediator)
        {
            _gameMediator = gameMediator;
            _uiMediator = uiMediator;
        }
        
        public override void Enter()
        {
            Subscribe();
            SceneManager.LoadScene(SceneName);
            Debug.Log("enter gameplay state");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == SceneName)
            {
                _uiMediator.Construct();
                _gameMediator.Construct();
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        private void Subscribe()
        {
            Application.quitting += Exit;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Unsubscribe()
        {
            Application.quitting -= Exit;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public override void Exit()
        {
            Unsubscribe();
            _uiMediator.Dispose();
            _gameMediator.Dispose();
            
            Debug.Log("exit application");
        }
        
        public class Factory : PlaceholderFactory<GameplayState> { }
    }
}