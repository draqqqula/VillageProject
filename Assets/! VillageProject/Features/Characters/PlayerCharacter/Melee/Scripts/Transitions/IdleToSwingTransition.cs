using System.Collections;
using UnityEngine;
using Zenject;
using R3;

public class IdleToSwingTransition : TransitionBase<IdleState>
{
    [Inject] private IAttackInput _attackInput;
    [Inject] private Stamina _stamina;
     
    public override void Construct(IdleState currentState)
    {
        _attackInput.IsHolding.Subscribe(HandleHolding).AddTo(this);
    }

    private void HandleHolding(bool value)
    {
        if (value && !_stamina.IsOnCooldown.CurrentValue)
        {
            Activate<SwingState>();
        }
    }
}