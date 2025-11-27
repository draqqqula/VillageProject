using System.Collections;
using R3;
using UnityEngine;
using Zenject;

public class AnyToGuardTransitionA<T> : TransitionBase<T>  where T : IState
{
    [Inject] private IBlockInput _blockInput;
    [Inject] private CoroutineHandler _coroutineHandler;
    [Inject] private InterruptToGuardTransitionChecker _interruptToGuardTransitionChecker;
    
    private bool _isBlockedInSwingState = false;
    
    public override void Construct(T currentState)
    {
        _interruptToGuardTransitionChecker.AddTo(this);
        _blockInput.IsHolding.Skip(1).Subscribe(OnBlockPressed).AddTo(this);
        _interruptToGuardTransitionChecker.CanInterrupt.Subscribe(TryInterrupt).AddTo(this);
    }

    private void OnBlockPressed(bool value)
    {
        _isBlockedInSwingState = value;
    }
    
    private void TryInterrupt(bool value)
    {
        _coroutineHandler.StartCoroutine(Delay(value));
    }
    
    private IEnumerator Delay(bool value) // временное решение из-за того, что AnimationWindow нельзя добавлять на 2 стейта
    {
        yield return null;
        InterruptTransition(value);
    }
    
    private void InterruptTransition(bool value)
    {
        if (value && _isBlockedInSwingState)
        {
            Activate<GuardState>();
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _isBlockedInSwingState = false;
    }
}