using R3;
using Zenject;

public class ThrustToGuardTransition : TransitionBase<ThrustState>
{
    [Inject(Id ="NotInterruptable")] private IAnimationWindowListener _interruptableWindow;
    [Inject] private IBlockInput _blockInput;
    [Inject] private Stamina _stamina;
    private InterruptToGuardTransitionChecker _interruptToGuardTransitionChecker;
    
    
    public override void Construct(ThrustState currentState)
    {
        _interruptToGuardTransitionChecker = new InterruptToGuardTransitionChecker(_interruptableWindow, _blockInput, _stamina);
        _interruptToGuardTransitionChecker.CanInterrupt.Subscribe(InterruptTransition).AddTo(this);
    }
    
    private void InterruptTransition(bool value)
    {
        if (value)
        {
            Activate<GuardState>();
        }
    }
}