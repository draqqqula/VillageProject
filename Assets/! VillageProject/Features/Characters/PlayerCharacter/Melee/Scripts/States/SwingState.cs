using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [Inject] private FirstPersonController _firstPersonController;
    [Inject] private MeleeControlsPresetManager _controlsPreset;
    [Inject] private MovementConfiguration _movementConfig;
    [Inject(Id = "Holding")] private IAnimationWindowListener _holdingWindowListener;
    [Inject(Id = "SlashAttack")] private IAnimationWindowListener _strikeWindowListener;
    private ReactiveProperty<Phase> _currentPhase = new ReactiveProperty<Phase>(Phase.Rise);
    private CompositeDisposable _shiftSubscription;
    private AttackDirection _direction = AttackDirection.None;
    private float _switchTimestamp = 0f;
    
    [Inject] MouseButtonsControlHandler _controlHandler;
    
    public ReadOnlyReactiveProperty<Phase> CurrentPhase => _currentPhase;
    public bool HoldingCancelled { get; private set; } = false;

    public override void OnEnter()
    {
        _stamina.ModifyRate(0).AddTo(this);
        _stamina.HoverAmount.Value += _config.StaminaCost;
        _indicatorController.IsLockIndicator = true;
        
        if (_controlsPreset.ChosenPreset.CurrentValue == 0)
        {
            _shiftInput.IsHolding.Subscribe(HandleShift).AddTo(this);
            SetDirection(_slashSeriesCounter.GetDirection());
        }
        else if (_controlsPreset.ChosenPreset.CurrentValue == 1)
        {
            _controlHandler.OnAttack += HandleAttack;
        }
        
        _attackBlendingController.ForceSnap();

        _animator.SetTrigger(AttackTrigger);

        _holdingWindowListener.OnEnter += HandleEnteredHolding;
        _strikeWindowListener.OnEnter += HandleExitedHolding;

        if (_controlsPreset.ChosenPreset.CurrentValue == 0)
        {
            _attackInput.IsHolding.Subscribe(HandleInputHolding).AddTo(this);
        }
        else if (_controlsPreset.ChosenPreset.CurrentValue == 1)
        {
            _attackInput.IsHolding.Subscribe(HandleInputHolding).AddTo(this);
            //_controlHandler.IsAttackHolding.Subscribe(HandleInputHolding).AddTo(this);
        }
    }

    private void HandleAttack(Vector2 direction)
    {
        Debug.Log("Handle Attack " + direction);
        if (direction == Vector2.zero) SetDirection(AttackDirection.Thrust);
        if (direction == Vector2.left) SetDirection(AttackDirection.LeftSwing);
        if (direction == Vector2.right) SetDirection(AttackDirection.RightSwing);
    }

    public override void OnExit()
    {
        _stamina.HoverAmount.Value -= _config.StaminaCost;
        _animator.SetBool(HoldingBoolean, false);
        _animator.ResetTrigger(ThrustTrigger);
        _indicatorController.IsLockIndicator = false;

        if (_controlsPreset.ChosenPreset.CurrentValue != 0)
        {
            _controlHandler.OnAttack -= HandleAttack;
        }

        _holdingWindowListener.OnEnter -= HandleEnteredHolding;
        _strikeWindowListener.OnEnter -= HandleExitedHolding;
        _shiftSubscription?.Dispose();
        SetDirection(AttackDirection.None);
    }

    private void HandleInputHolding(bool isPressed)
    {
        _animator.SetBool(HoldingBoolean, isPressed);
        if (!isPressed)
        {
            _shiftSubscription?.Dispose();
            _shiftSubscription = null;
        }
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

        var thrustValue = Mathf.Clamp01(1 - (vector.magnitude / (border * 2)));
        var leftValue = Mathf.Clamp01(-Mathf.Min(vector.x, 0) / (border * 2));
        var rightValue = Mathf.Clamp01(Mathf.Max(vector.x, 0) / (border * 2));

        var sum = thrustValue + leftValue + rightValue;

        if (sum == 0)
        {
            return;
        }

        var leftShare = leftValue / sum;
        var rightShare = rightValue / sum;
        var thrustShare = thrustValue / sum;

        _attackBlendingController.SetWeights(new Vector3(
            _movementConfig.Invert? rightShare : leftShare, 
            _movementConfig.Invert ? leftShare : rightShare, 
            thrustShare));

        if (thrustShare > Mathf.Max(leftShare, rightShare))
        {
            if (_direction != AttackDirection.Thrust)
            {
                TryPlaySwitchSound();
            }
            SetDirection(AttackDirection.Thrust);
            intensity = 1 - (vector.magnitude / border);
        }
        else if (leftShare > Mathf.Max(rightShare, thrustShare))
        {
            var targetDirection = _movementConfig.Invert ? AttackDirection.RightSwing : AttackDirection.LeftSwing;
            if (_direction != targetDirection)
            {
                TryPlaySwitchSound();
            }
            SetDirection(targetDirection);
            intensity = Mathf.Clamp01((Math.Abs(vector.x) - border) / (_config.MaxDeltaMagnitude - border));
        }
        else if (rightShare > Mathf.Max(leftShare, thrustShare))
        {
            var targetDirection = _movementConfig.Invert ? AttackDirection.LeftSwing : AttackDirection.RightSwing;
            if (_direction != targetDirection)
            {
                TryPlaySwitchSound();
            }
            SetDirection(targetDirection);
            intensity = Mathf.Clamp01((Math.Abs(vector.x) - border) / (_config.MaxDeltaMagnitude - border));
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

    private void TryPlaySwitchSound()
    {
        var time = Time.time;
        if (time > _switchTimestamp + _config.SwitchSoundCooldown)
        {
            _signalBus.Fire(new PlayAudioSignal<SwordCombatSounds>(SwordCombatSounds.Switch));
            _switchTimestamp = time;
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

            _firstPersonController.Sensitivity
                .AddMultiplier(_config.ShiftSensitivityMultiplier)
                .AddTo(_shiftSubscription);

            Disposable.Create(() =>
            {
                _signalBus.Fire(new ShowAttackDirectionSignal(false));
                _attackBlendingController.TransformWeights();
            })
            .AddTo(_shiftSubscription);


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

public interface IControlHandler
{
    public ReadOnlyReactiveProperty<bool> IsAttackHolding { get; }
    public ReadOnlyReactiveProperty<bool> IsBlockHolding { get; }
    public event Action<Vector2> OnAttack;
}

public class MouseButtonsControlHandler : IControlHandler, IDisposable
{
    public ReadOnlyReactiveProperty<bool> IsAttackHolding => _isAttackHolding;
    public ReadOnlyReactiveProperty<bool> IsBlockHolding => _isBlockHolding;

    private ReactiveProperty<bool> _isAttackHolding;
    private ReactiveProperty<bool> _isBlockHolding;
    public event Action<Vector2> OnAttack;
    
    private InputWithHolding _blockInput;
    private InputWithHolding _leftAttack;
    private InputWithHolding _rightAttack;
    
    private CoroutineHandler _coroutineHandler;
    private CompositeDisposable _disposables = new CompositeDisposable();
    
    public MouseButtonsControlHandler(InputWithHolding leftAttack, InputWithHolding rightAttack, InputWithHolding blockInput, CoroutineHandler coroutineHandler)
    {
        _isAttackHolding = new ReactiveProperty<bool>();
        _isBlockHolding = new ReactiveProperty<bool>();
        
        _blockInput = blockInput;
        _blockInput.IsHolding.Skip(1).Subscribe(HandleBlock).AddTo(_disposables);
        
        _leftAttack = leftAttack;
        _leftAttack.IsHolding.Skip(1).Subscribe(HandleLeftAttack).AddTo(_disposables);
        
        _rightAttack = rightAttack;
        _rightAttack.IsHolding.Skip(1).Subscribe(HandleRightAttack).AddTo(_disposables);
        Debug.Log("Subscribed");

        _coroutineHandler = coroutineHandler;
    }

    private void HandleLeftAttack(bool value)
    {
        Debug.Log("Left attack");
        HandleAttack(value);
        if (value) _coroutineHandler.StartCoroutine(AttackDelay(Vector2.left));
    }
    
    private void HandleRightAttack(bool value)
    {
        Debug.Log("Right attack");
        HandleAttack(value);
        if (value) _coroutineHandler.StartCoroutine(AttackDelay(Vector2.right));
    }

    private void HandleBlock(bool value)
    {
        _isBlockHolding.Value = value;
    }

    private void HandleAttack(bool value)
    {
        _isAttackHolding.Value = value;
    }

    private IEnumerator AttackDelay(Vector2 direction)
    {
        yield return null;
        OnAttack?.Invoke(direction);
        Debug.Log("Attack");
        HandleAttack(false);
    }

    public void Dispose()
    {
        Debug.Log("Disposing");
        _disposables.Dispose();
    }
}