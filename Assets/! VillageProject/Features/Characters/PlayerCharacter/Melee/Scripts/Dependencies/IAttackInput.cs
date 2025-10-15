using R3;
using System.Collections;
using UnityEngine;

public interface IAttackInput
{
    public ReadOnlyReactiveProperty<bool> IsHolding { get; }
    public ReadOnlyReactiveProperty<float> LastStarted { get; }
    public ReadOnlyReactiveProperty<float> LastEnded { get; }
    public float GetUnscaledTimeSinceLastStarted();
    public float GetUnscaledTimeSinceLastEnded();
}