using System.Collections;
using R3;
using UnityEngine;
using Zenject;

public class GuardToIdleTransition : TransitionBase<GuardState>
{
    private GuardState _guardState;
    
    public override void Construct(GuardState currentState)
    {
        _guardState = currentState;
        _guardState.ShieldValue.Subscribe(HandleReleased).AddTo(this);
    }
    
    private void HandleReleased(float value)
    {
        if (value == 0)
        {
            Activate<IdleState>();
        }
    }
}