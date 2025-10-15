using R3;
using System.Collections;
using UnityEngine;
using Zenject;

public class SwingToThrustTransition : TransitionBase<SwingState>
{
    [Inject] private IAttackInput _attackInput;
    private SwingState _state;

    public override void Construct(SwingState currentState)
    {
        _state = currentState;
        currentState.CurrentPhase.Subscribe(HandlePhaseChanged).AddTo(this);
    }

    private void HandlePhaseChanged(SwingState.Phase phase)
    {
        if (_attackInput.IsHolding.CurrentValue && phase == SwingState.Phase.Strike && !_state.HoldingCancelled)
        {
            Activate<ThrustState>();
        }
    }
}