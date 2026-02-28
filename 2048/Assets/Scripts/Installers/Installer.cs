using Gameplay;
using Gameplay.Cube;
using Services;
using StateMachine.Global;
using StateMachine.Global.States;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class Installer : MonoInstaller
    {
        [SerializeField] private CubeConfig _cubeConfig;
        
        public override void InstallBindings()
        {
            BindGlobalStateMachine();
            BindGameplay();
            BindServices();
        }

        private void BindGlobalStateMachine()
        {
            Container.Bind<GlobalStateMachine>().AsSingle();
            Container.BindFactory<GlobalStateMachine, BootState, BootState.Factory>().AsSingle();
            Container.BindFactory<GameplayState, GameplayState.Factory>(); 
        }

        private void BindGameplay()
        {
            Container.Bind<GenericFactory>().AsSingle();
            Container.Bind<GameMediator>().AsSingle();
            Container.Bind<CubeSpawner>().AsSingle();
            Container.Bind<CubeConfig>().FromScriptableObject(_cubeConfig).AsSingle();
            
        }
        
        private void BindServices()
        {
            Container.Bind<IAssetProviderService>().To<AssetProviderService>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
            Container.Bind<MergeService>().AsSingle();
            Container.Bind<ScoreService>().AsSingle();
            Container.Bind<AutoMergeService>().AsSingle();
            Container.Bind<CameraShakeService>().AsSingle();
        }
    }
}