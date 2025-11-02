using R3;
using UnityEngine;
using Zenject;

public class IdleToGuardTransition : TransitionBase<IdleState>
{
    [Inject] private IBlockInput _blockInput;
     
    public override void Construct(IdleState currentState)
    {
        _blockInput.IsHolding.Subscribe(HandleHolding).AddTo(this);
    }

    private void HandleHolding(bool value)
    {
        if (value)
        {
            Activate<GuardState>();
        }
    }
}