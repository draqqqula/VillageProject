using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SwingState : StateBase<SwingState>
{
    public enum Phase
    {
        Rise,
        Holding,
        Strike
    }

    private const string AttackTrigger = "Attack";

    [Inject] private Stamina _stamina;
    [Inject] private SwingConfiguration _config;
    [Inject] private CoroutineHandler _coroutineHandler;
    [Inject] private Animator _animator;
    [Inject] private IAttackInput _attackInput;
    [Inject(Id = "Holding")] private IAnimationWindowListener _holdingWindowListener;
    private Coroutine _holding;
    private ReactiveProperty<Phase> _currentPhase = new ReactiveProperty<Phase>(Phase.Rise);

    public ReadOnlyReactiveProperty<Phase> CurrentPhase => _currentPhase;
    public bool HoldingCancelled { get; private set; } = false;

    public override void OnEnter()
    {
        _stamina.ModifyRate(0).AddTo(this);
        _holdingWindowListener.OnEnter += HandleEnteredSlowdown;
        _holdingWindowListener.OnExit += HandleExitedSlowdown;

        _stamina.HoverAmount.Value += _config.StaminaCost;
        _animator.SetTrigger(AttackTrigger);

        _attackInput.IsHolding.Subscribe(HandleHoldingCancelled).AddTo(this);
    }

    public override void OnExit()
    {
        _stamina.HoverAmount.Value -= _config.StaminaCost;
        _animator.speed = 1;

        _holdingWindowListener.OnEnter -= HandleEnteredSlowdown;
        _holdingWindowListener.OnExit -= HandleExitedSlowdown;

        if (_holding != null)
        {
            _coroutineHandler.StopCoroutine(_holding);
        }
    }

    private void HandleEnteredSlowdown()
    {
        _currentPhase.Value = Phase.Holding;
        if (!HoldingCancelled)
        {
            _coroutineHandler.StartCoroutine(DelayEnterHolding());
        }
    }

    private void HandleExitedSlowdown()
    {
        _currentPhase.Value = Phase.Strike;
    }

    private void HandleHoldingCancelled(bool isPressed)
    {
        if (!isPressed)
        {
            HoldingCancelled = true;
        }
    }

    private IEnumerator DelayEnterHolding()
    {
        _currentPhase.Value = Phase.Holding;

        while (_holdingWindowListener.IsActive.CurrentValue && _attackInput.IsHolding.CurrentValue && !HoldingCancelled)
        {
            _animator.speed = _config.SlowdownCurve.Evaluate(_holdingWindowListener.Progress);

            yield return new WaitForFixedUpdate();
        }
        _animator.speed = 1;
    }
}