using System;
using R3;
using UnityEngine;
using Zenject;

public class GuardToIdleBreakingTransition : TransitionBase<GuardState>, IDisposable
{
    [Inject] private Stamina _stamina;
    [Inject] private Animator _animator;
    [Inject(Id = "BreakingBlock")] private IAnimationWindowListener _breakingWindowListener; 
    [Inject] private StateManager _stateManager;
    
    public override void Construct(GuardState currentState)
    {
        _stamina.Value.Subscribe(HandleReleased).AddTo(this);
        _breakingWindowListener.OnEnter += ActivateIdle;
    }

    private void HandleReleased(float value)
    {
        if (!_stateManager.IsTransitionSubscribed.CurrentValue) return;
        
        if (value == 0 && _stateManager.Current.CurrentValue is GuardState)
        {
            _animator.SetTrigger("Break");
        }
    }

    private void ActivateIdle()
    {
        Activate<IdleState>();
    }

    public void Dispose()
    {
        _breakingWindowListener.OnEnter -= ActivateIdle;
    }
}