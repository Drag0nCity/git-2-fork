using Zenject;

public class SaveInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<SaveService>()
            .AsSingle()
            .NonLazy();
    }
}
