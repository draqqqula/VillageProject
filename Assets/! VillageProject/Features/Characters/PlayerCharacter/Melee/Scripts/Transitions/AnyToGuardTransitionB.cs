using System;
using R3;
using UnityEngine;
using Zenject;

public class AnyToGuardTransitionB : TransitionBase<SlashState>
{
    [Inject] private InterruptToGuardTransitionChecker _interruptToGuardChecker;

    public override void Construct(SlashState currentState)
    {
        _interruptToGuardChecker.AddTo(this);
        _interruptToGuardChecker.CanInterrupt.Subscribe(InterruptTransition).AddTo(this);
    }
    
    private void InterruptTransition(bool value)
    {
        if (value)
        {
            Activate<GuardState>();
        }
    }
}