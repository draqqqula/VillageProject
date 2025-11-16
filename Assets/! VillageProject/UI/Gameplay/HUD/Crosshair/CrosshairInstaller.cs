using System.Collections;
using UnityEngine;
using Zenject;

public class CrosshairInstaller : MonoInstaller
{
    [SerializeField] private DirectionDisplay _directionDisplay;

    public override void InstallBindings()
    {
        Container.DeclareSignal<SetAttackDirectionSignal>();
        Container.DeclareSignal<ShowAttackDirectionSignal>();
        Container.DeclareSignal<AttackDirectionIntensitySignal>();
        Container.BindInstance(_directionDisplay).AsSingle();
        Container.BindInterfacesAndSelfTo<SetAttackDirectionSignalListener>().AsSingle();
    }
}