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

    [Inject] private SignalBus _signalBus;
    [Inject] private AttackBlendingController _attackBlendingController;
    [Inject] private DiContainer _container;
    [Inject] private Stamina _stamina;
    [Inject] private SwingConfiguration _config;
    [Inject] private IndicatorController _indicatorController;
    [Inject] private Animator _animator;
    [Inject] private IAttackInput _attackInput;
    [Inject] private IShiftInput _shiftInput;
    [Inject] private SlashSeriesCounter _slashSeriesCounter;
    [Inject(Id = "Holding")] private IAnimationWindowListener _holdingWindowListener;
    [Inject(Id = "SlashAttack")] private IAnimationWindowListener _strikeWindowListener;
    private ReactiveProperty<Phase> _currentPhase = new ReactiveProperty<Phase>(Phase.Rise);
    private CompositeDisposable _shiftSubscription;
    private AttackDirection _direction = AttackDirection.None;

    public ReadOnlyReactiveProperty<Phase> CurrentPhase => _currentPhase;
    public bool HoldingCancelled { get; private set; } = false;

    public override void OnEnter()
    {
        _stamina.ModifyRate(0).AddTo(this);
        _stamina.HoverAmount.Value += _config.StaminaCost;
        _indicatorController.IsLockIndicator = true;

        SetDirection(_slashSeriesCounter.GetDirection());
        _shiftInput.IsHolding.Subscribe(HandleShift).AddTo(this);
        _attackBlendingController.ForceSnap();

        _animator.SetTrigger(AttackTrigger);

        _holdingWindowListener.OnEnter += HandleEnteredHolding;
        _strikeWindowListener.OnEnter += HandleExitedHolding;

        _attackInput.IsHolding.Subscribe(SetAnimatorHolding).AddTo(this);
    }

    public override void OnExit()
    {
        _stamina.HoverAmount.Value -= _config.StaminaCost;
        _animator.SetBool(HoldingBoolean, false);
        _animator.ResetTrigger(ThrustTrigger);
        _indicatorController.IsLockIndicator = false;

        _holdingWindowListener.OnEnter -= HandleEnteredHolding;
        _strikeWindowListener.OnEnter -= HandleExitedHolding;
        _shiftSubscription?.Dispose();
        SetDirection(AttackDirection.None);
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

    private void HandleAttackVector(Vector2 vector)
    {
        var border = _config.ThrustBorder;
        var intensity = 0f;
        if (Mathf.Abs(vector.magnitude) <= border)
        {
            SetDirection(AttackDirection.Thrust);
            intensity = 1 - (vector.magnitude / border);
        }
        else if (vector.y < 0)
        {
            SetDirection(AttackDirection.LeftSwing);
            intensity = Mathf.Clamp01((Math.Abs(vector.y) - border) / (_config.MaxDeltaMagnitude - border));
        }
        else if (vector.y > 0)
        {
            SetDirection(AttackDirection.RightSwing);
            intensity = Mathf.Clamp01((Math.Abs(vector.y) - border) / (_config.MaxDeltaMagnitude - border));
        }
        _signalBus.Fire(new AttackDirectionIntensitySignal(intensity));
    }

    private void SetDirection(AttackDirection direction)
    {
        if (_direction != direction)
        {
            _attackBlendingController.Direction.Value = direction;
            _direction = direction;
        }
    }

    private void HandleShift(bool value)
    {
        if (value)
        {
            if (_shiftSubscription != null)
            {
                return;
            }
            _shiftSubscription = new CompositeDisposable();

            _attackBlendingController.Direction.Subscribe(DisplayDirection);
            var deltaHandler = _container.Resolve<CursorDeltaHandler>();
            deltaHandler.AddTo(_shiftSubscription);
            deltaHandler.Velocity.Subscribe(HandleAttackVector).AddTo(_shiftSubscription);
            Disposable.Create(() => _signalBus.Fire(new ShowAttackDirectionSignal(false))).AddTo(_shiftSubscription);

            _signalBus.Fire(new ShowAttackDirectionSignal(true));
        }
        else
        {
            _shiftSubscription?.Dispose();
            _shiftSubscription = null;
        }
    }

    private void DisplayDirection(AttackDirection direction)
    {
        _signalBus.Fire(new SetAttackDirectionSignal(direction));
    }
}