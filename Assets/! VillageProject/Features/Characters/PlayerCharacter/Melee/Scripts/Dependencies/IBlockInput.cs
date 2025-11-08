using R3;
using UnityEngine;

public interface IBlockInput
{
    public ReadOnlyReactiveProperty<bool> IsHolding { get; }
    public ReadOnlyReactiveProperty<float> LastStarted { get; }
    public ReadOnlyReactiveProperty<float> LastEnded { get; }
    public ReactiveProperty<float> CurrentHoldTime { get; }
    
    public float GetUnscaledTimeSinceLastStarted();
    public float GetUnscaledTimeSinceLastEnded();
}
