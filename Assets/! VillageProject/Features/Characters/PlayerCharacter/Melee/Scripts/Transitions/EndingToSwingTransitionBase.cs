using R3;
using System.Collections;
using UnityEngine;
using Zenject;

public abstract class EndingToSwingTransitionBase<T> : TransitionBase<T> where T : EndingState<T>
{
    [Inject] protected IAttackInput AttackInput { get; private set; }
    [Inject] protected Stamina Stamina { get; private set; }
    [Inject] protected SwingConfiguration SlashConfiguration { get; private set; }
    protected T CurrentState { get; private set; }

    public override void Construct(T currentState)
    {
        CurrentState = currentState;
        currentState.HasEnded.Subscribe(_ => HandleEnded()).AddTo(this);
        AttackInput.IsHolding.Subscribe(_ => HandleEnded()).AddTo(this);
    }

    private void HandleEnded()
    {
        if (ShouldActivate())
        {
            Activate<SwingState>();
        }
    }

    protected abstract bool ShouldActivate();
}