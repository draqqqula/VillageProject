using System.Collections;
using UnityEngine;
using Zenject;
using R3;

public class SwingToSlashTransition : TransitionBase<SwingState>
{
    [Inject] private IAttackInput _attackInput;
    [Inject] private AttackBlendingController _attackBlendingController;
    private SwingState _state;

    public override void Construct(SwingState currentState)
    {
        _state = currentState;
        _attackInput.IsHolding.Subscribe(_ => HandleReleased()).AddTo(this);
        _state.CurrentPhase.Subscribe(_ => HandleReleased()).AddTo(this);
    }

    private void HandleReleased()
    {
        if (!_attackInput.IsHolding.CurrentValue && _state.CurrentPhase.CurrentValue == SwingState.Phase.Strike)
        {
            var direction = _attackBlendingController.Direction.CurrentValue;
            var slash = Activate<SlashState>();
            slash.AttackDirection = direction;
        }
    }
}