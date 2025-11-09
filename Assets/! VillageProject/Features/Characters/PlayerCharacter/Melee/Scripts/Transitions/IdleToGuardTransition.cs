using R3;
using UnityEngine;
using Zenject;

public class IdleToGuardTransition : TransitionBase<IdleState>
{
    [Inject] private IBlockInput _blockInput;
    [Inject] private StateManager _stateManager;
    
    private bool _isActivated = false;
     
    public override void Construct(IdleState currentState)
    {
        _blockInput.IsHolding.Subscribe(ctx => HandleStartHolding()).AddTo(this);
        _stateManager.IsTransitionSubscribed.Subscribe(ctx => HandleStartHolding()).AddTo(this);
    }
    
    private void HandleStartHolding()
    {
        if (_blockInput.IsHolding.CurrentValue && _stateManager.IsTransitionSubscribed.CurrentValue)
        {
            if (_stateManager.Current.CurrentValue is GuardState) return;
            Activate<GuardState>();
        }
    }
}