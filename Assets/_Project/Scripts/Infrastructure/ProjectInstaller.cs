using UnityEngine.InputSystem;
using Zenject;

namespace TPShooter.Infrastructure
{
    public sealed class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var inputActions = new TPShooterInputActions();

            Container.BindInstance(inputActions)
                .AsSingle();

            Container.Bind<IInputService>()
                .To<InputService>()
                .AsSingle();
        }
    }
}