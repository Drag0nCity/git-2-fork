using TPShooter.Player;
using UnityEngine;
using Zenject;

namespace TPShooter.Infrastructure
{
    public sealed class GameInstaller : MonoInstaller
    {
        [SerializeField] private PlayerController playerController;

        public override void InstallBindings()
        {
            Container.Bind<PlayerController>()
                .FromInstance(playerController)
                .AsSingle();

            Container.Bind<GameEndController>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}