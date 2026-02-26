using R3;

public interface IStartWaveInput
{
    public ReadOnlyReactiveProperty<bool> IsHolding { get; }
}