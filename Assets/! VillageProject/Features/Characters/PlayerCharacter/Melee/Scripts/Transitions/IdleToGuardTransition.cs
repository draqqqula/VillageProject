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
        _blockInput.IsHolding.Subscribe(ctx => Release()).AddTo(this);
    }
    
    private void HandleStartHolding()
    {
        if (_isActivated) return;
        
        if (_blockInput.IsHolding.CurrentValue && _stateManager.IsTransitionSubscribed.CurrentValue)
        {
            _isActivated = true;
            Activate<GuardState>();
        }
    }

    private void Release()
    {
        if (!_blockInput.IsHolding.CurrentValue) _isActivated = false;
    }
}