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
    private const string ThrustTrigger = "Thrust";
    private const string HoldingBoolean = "Holding";
    public override StateType StateType => StateType.Swing;

    [Inject] private Stamina _stamina;
    [Inject] private SwingConfiguration _config;
    [Inject] private Animator _animator;
    [Inject] private IAttackInput _attackInput;
    [Inject] private IShiftInput _shiftInput;
    [Inject(Id = "Holding")] private IAnimationWindowListener _holdingWindowListener;
    [Inject(Id = "SlashAttack")] private IAnimationWindowListener _strikeWindowListener;
    private ReactiveProperty<Phase> _currentPhase = new ReactiveProperty<Phase>(Phase.Rise);

    public ReadOnlyReactiveProperty<Phase> CurrentPhase => _currentPhase;
    public bool HoldingCancelled { get; private set; } = false;

    public override void OnEnter()
    {
        _stamina.ModifyRate(0).AddTo(this);
        _stamina.HoverAmount.Value += _config.StaminaCost;

        HandleShift(_shiftInput.IsHolding.CurrentValue);

        _animator.SetTrigger(AttackTrigger);

        _holdingWindowListener.OnEnter += HandleEnteredHolding;
        _strikeWindowListener.OnEnter += HandleExitedHolding;

        _attackInput.IsHolding.Subscribe(SetAnimatorHolding).AddTo(this);
        _shiftInput.IsHolding.Subscribe(HandleShift).AddTo(this);
    }

    public override void OnExit()
    {
        _stamina.HoverAmount.Value -= _config.StaminaCost;
        _animator.SetBool(HoldingBoolean, false);
        _animator.ResetTrigger(ThrustTrigger);

        _holdingWindowListener.OnEnter -= HandleEnteredHolding;
        _strikeWindowListener.OnEnter -= HandleExitedHolding;
    }

    private void SetAnimatorHolding(bool isPressed)
    {
        _animator.SetBool(HoldingBoolean, isPressed);
    }

    private void HandleEnteredHolding()
    {
        _currentPhase.Value = Phase.Holding;
    }

    private void HandleExitedHolding()
    {
        _currentPhase.Value = Phase.Strike;
    }

    private void HandleShift(bool value)
    {
        if (value)
        {
            _animator.SetTrigger(ThrustTrigger);
        }
    }
}