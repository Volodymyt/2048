using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using StateMachine.Base;

namespace StateMachine.Global.States
{
    public class GameplayState : State
    {
        private const string SceneName = "Game Scene";

        private readonly GameMediator _gameMediator;

        public GameplayState(GameMediator gameMediator)
        {
            _gameMediator = gameMediator;
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
            
            Debug.Log("exit application");
        }


        public class Factory : PlaceholderFactory<GameplayState> { }
    }
}