using R3;
using UnityEngine;
using Zenject;

public class GuardToSwingTransition : TransitionBase<GuardState>
{
    [Inject] private IAttackInput _attackInput;
    [Inject] private Stamina _stamina;
    [Inject] protected SwingConfiguration _slashConfiguration;
    private bool _isSwinged;
    
    private GuardState _guardState;
    
    public override void Construct(GuardState currentState)
    {
        _guardState = currentState;
        _guardState.CanInterrupt.Subscribe(TryTransition).AddTo(this);
        _attackInput.IsHolding.Subscribe(TryTransition).AddTo(this);
    }

    public bool CanAttack()
    {
        return _guardState.CanInterrupt.CurrentValue && !_stamina.IsOnCooldown.CurrentValue;
    }
    
    private bool IsAttackInput()
    {
        return (_attackInput.IsHolding.CurrentValue
                || _attackInput.GetUnscaledTimeSinceLastEnded() < _slashConfiguration.InputBufferADuration);
    }
    
    private void TryTransition(bool value)
    {
        if (_isSwinged) return;
        
        if (CanAttack() && IsAttackInput())
        {
            _isSwinged = true;
            Activate<SwingState>();
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _isSwinged = false;
    }
}