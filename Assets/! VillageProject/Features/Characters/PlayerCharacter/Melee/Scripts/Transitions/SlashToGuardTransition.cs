using System;
using R3;
using Zenject;

public class SlashToGuardTransition : TransitionBase<SlashState>
{
    [Inject(Id ="NotInterruptable")] private IAnimationWindowListener _interruptableWindow;
    [Inject] private IBlockInput _blockInput;
    [Inject] private Stamina _stamina;
    private InterruptToGuardTransitionChecker _interruptToGuardChecker;

    public override void Construct(SlashState currentState)
    {
        _interruptToGuardChecker = new InterruptToGuardTransitionChecker(_interruptableWindow, _blockInput, _stamina);
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