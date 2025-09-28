using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;

public class PlayerToSceneInstaller : MonoInstaller
{
    [Inject] Stamina Stamina;
    [Inject] Adrenaline Adrenaline;
    public override void InstallBindings()
    {
        Container.BindInstance(Stamina).AsSingle();
        Container.BindInstance(Adrenaline).AsSingle();
    }
}