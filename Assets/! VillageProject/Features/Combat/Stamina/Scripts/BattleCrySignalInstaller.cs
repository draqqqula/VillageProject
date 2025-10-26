using Zenject;

public class BattleCrySignalInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.DeclareSignal<BattleCry.BattleCryPerformedSignal>();
        Container.DeclareSignal<BattleCry.BattleCryCooldownFinishedSignal>();
        Container.DeclareSignal<BattleCry.BattleCryFinishedSignal>();
    }
}