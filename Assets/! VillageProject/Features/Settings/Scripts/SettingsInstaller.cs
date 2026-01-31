using UnityEngine;
using Zenject;

public class SettingsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<SettingsInstaller>();
        Container.Bind<SettingsProvider>().AsSingle();
        base.InstallBindings();
    }
}
