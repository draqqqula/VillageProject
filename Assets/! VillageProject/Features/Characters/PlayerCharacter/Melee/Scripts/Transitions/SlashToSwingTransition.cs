using R3;
using System.Collections;
using UnityEngine;
using Zenject;

public class SlashToSwingTransition : TransitionBase<SlashState>
{
    [Inject(Id ="AttackInterruptable")] private IAnimationWindowListener _interruptableWindow;
    [Inject] private IAttackInput _attackInput;
    [Inject] private Stamina _stamina;
    [Inject] protected SwingConfiguration _slashConfiguration;

    public override void Construct(SlashState currentState)
    {
        _interruptableWindow.IsActive.Subscribe(TryTransition).AddTo(this);
        _attackInput.IsHolding.Subscribe(TryTransition).AddTo(this);
    }

    private bool IsAttackInput()
    {
        return (_attackInput.IsHolding.CurrentValue
            || _attackInput.GetUnscaledTimeSinceLastEnded() < _slashConfiguration.InputBufferADuration);
    }

    private bool CanAttack()
    {
        return _interruptableWindow.IsActive.CurrentValue && !_stamina.IsOnCooldown.CurrentValue;
    }

    private void TryTransition(bool value)
    {
        if (CanAttack() && IsAttackInput())
        {
            Activate<SwingState>();
        }
    }
}