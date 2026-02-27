using Gameplay;
using Gameplay.Cube;
using Services;
using StateMachine.Global;
using StateMachine.Global.States;
using Zenject;

namespace Installers
{
    public class Installer : MonoInstaller
    {
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
            Container.Bind<GameMediator>().AsSingle();
            Container.Bind<CubeSpawner>().AsSingle();
        }
        
        private void BindServices()
        {
            Container.Bind<IAssetProviderService>().To<AssetProviderService>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
        }
    }
}