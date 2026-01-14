using System.Collections;
using R3;
using UnityEngine;
using Zenject;

public class GuardToSwingTransition : TransitionBase<GuardState>
{
    [Inject] private IAttackInput _attackInput;
    [Inject] private Stamina _stamina;
    [Inject] protected SwingConfiguration _slashConfiguration;
    [Inject] private StateManager _stateManager;
    
    [Inject] private Animator _animator;
    [Inject] private CoroutineHandler _coroutineHandler;
    private Coroutine _coroutine;
    
    private bool _isSwinged;
    private bool _isAttackedInGuardState;
    
    private GuardState _guardState;
    
    public override void Construct(GuardState currentState)
    {
        _attackInput.IsHolding.Skip(1).Subscribe(OnAttackPressed).AddTo(this);
        _guardState = currentState;
        _guardState.CanInterrupt.Subscribe(TryTransition).AddTo(this);
        _attackInput.IsHolding.Subscribe(TryTransition).AddTo(this);
    }
    
    private void OnAttackPressed(bool value)
    {
        _isAttackedInGuardState = value;
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
        if (_isSwinged || !_stateManager.IsTransitionSubscribed.CurrentValue) return;
        
        if (CanAttack() && IsAttackInput() && _isAttackedInGuardState)
        {
            _isSwinged = true;
            if (_coroutine != null) _coroutineHandler.StopCoroutine(_coroutine);
            _coroutine = _coroutineHandler.StartCoroutine(ReleaseShieldParamRoutine());
            
            Activate<SwingState>();
        }
    }
    
    private IEnumerator ReleaseShieldParamRoutine() // Для плавного прерывания в SwingState
    {
        _guardState.IsReleaseShieldAfterExit = false;
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsName("Hold"));
        _animator.SetFloat("Shield", 0);
    }
    
    public override void Dispose()
    {
        base.Dispose();
        _isSwinged = false;
        _isAttackedInGuardState = false;
    }
}