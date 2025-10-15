using System.Collections;
using UnityEngine;

public class EndingToSwingTransitionB<T> : EndingToSwingTransitionBase<T> where T : EndingState<T>
{
    protected override bool ShouldActivate()
    {
        return CurrentState.HasEnded.CurrentValue 
            && AttackInput.GetUnscaledTimeSinceLastStarted() < SlashConfiguration.InputBufferBThreshold
            && !Stamina.IsOnCooldown.CurrentValue;
    }
}