using Gameplay;
using Gameplay.Configs;
using Gameplay.Cube;
using Services;
using StateMachine.Global;
using StateMachine.Global.States;
using UI;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class Installer : MonoInstaller
    {
        [SerializeField] private CubeConfig cubeConfig;
        [SerializeField] private BoardConfig boardConfig;
        
        public override void InstallBindings()
        {
            BindGlobalStateMachine();
            BindGameplay();
            BindServices();
            BindUI();
        }

        private void BindGlobalStateMachine()
        {
            Container.Bind<GlobalStateMachine>().AsSingle();
            Container.BindFactory<GlobalStateMachine, BootState, BootState.Factory>().AsSingle();
            Container.BindFactory<GameplayState, GameplayState.Factory>(); 
        }

        private void BindGameplay()
        {
            Container.Bind<GameMediator>().AsSingle();
            Container.Bind<CubeSpawner>().AsSingle();
            Container.Bind<CubeConfig>().FromScriptableObject(cubeConfig).AsSingle();
            Container.Bind<BoardConfig>().FromScriptableObject(boardConfig).AsSingle();
            Container.Bind<CameraShake>().AsSingle();
            Container.Bind<MergeSystem>().AsSingle();
            Container.Bind<ScoreSystem>().AsSingle();
            Container.Bind<AutoMerge>().AsSingle();
        }

        private void BindUI()
        {
            Container.Bind<UIMediator>().AsSingle();
        }
        
        private void BindServices()
        {
            Container.Bind<IAssetProviderService>().To<AssetProviderService>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
            Container.Bind<GenericFactory>().AsSingle();

        }
    }
}