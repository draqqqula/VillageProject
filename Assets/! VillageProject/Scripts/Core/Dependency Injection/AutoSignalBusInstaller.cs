using System.Collections;
using UnityEngine;
using Zenject;

public class AutoSignalBusInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
    }
}