using System.Collections;
using R3;
using UnityEngine;
using Zenject;

public class GuardToIdleTransition : TransitionBase<GuardState>
{
    private GuardState _guardState;
    [Inject] StateManager _stateManager;
    
    public override void Construct(GuardState currentState)
    {
        _guardState = currentState;
        _guardState.ShieldValue.Subscribe(HandleReleased).AddTo(this);
    }
    
    private void HandleReleased(float value)
    {
        if (value == 0)
        {
            if (!_stateManager.IsTransitionSubscribed.CurrentValue || _stateManager.Current.CurrentValue is IdleState) return;
            Activate<IdleState>();
        }
    }
}