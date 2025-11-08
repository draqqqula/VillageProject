using System.Collections;
using UnityEngine;

public class EndingToSwingTransitionA<T> : EndingToSwingTransitionBase<T> where T : EndingState<T>
{
    protected override bool ShouldActivate()
    {
        return CurrentState.HasEnded.CurrentValue && (AttackInput.IsHolding.CurrentValue
            || AttackInput.GetUnscaledTimeSinceLastEnded() < SlashConfiguration.InputBufferADuration)
            && !Stamina.IsOnCooldown.CurrentValue;
    }
}