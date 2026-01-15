using UnityEngine;
using Zenject;

public class BattleCrySignalInstaller : MonoInstaller
{
    [SerializeField] private BattleCry _battleCry;
    
    public override void InstallBindings()
    {
        Container.Bind<BattleCry>().FromInstance(_battleCry).AsSingle();
        Container.BindInterfacesAndSelfTo<BattleCrySignalInvoker>().AsSingle();
        
        Container.DeclareSignal<BattleCrySignalInvoker.BattleCryStartedSignal>();
        Container.DeclareSignal<BattleCrySignalInvoker.BattleCryStartCooldownSignal>();
        Container.DeclareSignal<BattleCrySignalInvoker.BattleCryCooldownFinishedSignal>();
        Container.DeclareSignal<BattleCrySignalInvoker.BattleCryFinishedSignal>();
    }
}