using System.Collections;
using UnityEngine;
using Zenject;

public class PlayerHealthSignalInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.DeclareSignal<PlayerHealthSignalInvoker.PlayerInitializeHealthSignal>();
        Container.DeclareSignal<PlayerHealthSignalInvoker.PlayerDeathSignal>();
        Container.DeclareSignal<PlayerHealthSignalInvoker.PlayerHurtSignal>();
        Container.DeclareSignal<PlayerHealthSignalInvoker.PlayerResetMaxHealthSignal>();
        Container.DeclareSignal<PlayerHealthSignalInvoker.PlayerHealthChangedSignal>();
    }
}