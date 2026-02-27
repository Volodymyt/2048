using System.ComponentModel;
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
        }

        private void BindGlobalStateMachine()
        {
            Container.Bind<GlobalStateMachine>().AsSingle();
            Container.BindFactory<GlobalStateMachine, BootState, BootState.Factory>().AsSingle();
            Container.BindFactory<GameplayState, GameplayState.Factory>(); 
        }
    }
}