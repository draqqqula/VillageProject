using System.Collections;
using R3;
using Zenject;

public class SwingToGuardTransition : TransitionBase<SwingState>
{
    [Inject(Id ="NotInterruptable")] private IAnimationWindowListener _interruptableWindow;
    [Inject] private IBlockInput _blockInput;
    [Inject] private Stamina _stamina;
    [Inject] private CoroutineHandler _coroutineHandler;
    private InterruptToGuardTransitionChecker _interruptToGuardTransitionChecker;
    
    public override void Construct(SwingState currentState)
    {
        _interruptToGuardTransitionChecker = new InterruptToGuardTransitionChecker(_interruptableWindow, _blockInput,  _stamina);
        _interruptToGuardTransitionChecker.CanInterrupt.Subscribe(TryInterrupt).AddTo(this);
    }
    
    private void TryInterrupt(bool value)
    {
        _coroutineHandler.StartCoroutine(Delay());
    }
    
    private IEnumerator Delay() // временное решение из-за того, что AnimationWindow нельзя добавлять на 2 стейта
    {
        yield return null;
        InterruptTransition(false);
    }
    
    private void InterruptTransition(bool value)
    {
        if (value)
        {
            Activate<GuardState>();
        }
    }
}