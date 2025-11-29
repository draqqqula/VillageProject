using System.Collections;
using R3;
using UnityEngine;
using Zenject;

public sealed class GuardState : StateBase<GuardState>
{
    private const string HitParam = "Hit";
    private const string ShieldParam = "Shield";
    private const string BlockParam = "Block";
    
    public override StateType StateType => StateType.Guard;
    
    public ReactiveProperty<float> ShieldValue { get; private set; }
    private bool _isShieldActive;
    
    [Inject] private GuardConfiguration _guardConfiguration;
    
    [Inject] private CoroutineHandler _coroutineHandler;
    private Coroutine _coroutine;
    
    [Inject] private Animator _animator;
    [Inject] private Stamina _stamina;
    [Inject] private IBlockInput _blockInput;
    
    [Inject] private HorizontalMovement _horizontalMovement;
    private FloatMultiplierModifier _floatModifier;
    
    [Inject] private HitboxEvent _shieldHitboxEvent;
    private HitRegistrar _registrar;
    
    private ReactiveProperty<bool> _canInterrupt = new ReactiveProperty<bool>(true);
    public ReadOnlyReactiveProperty<bool> CanInterrupt => _canInterrupt;
    public bool IsReleaseShieldAfterExit {private get; set; }
    
    public override void OnEnter()
    {
        _registrar = new HitboxHitRegistrar(_shieldHitboxEvent);
        _registrar.OnHit += OnHit;
        
        _stamina.ModifyRate(_guardConfiguration.StaminaFillModifier).AddTo(this);
        _animator.SetBool(BlockParam, true);
        
        ShieldValue = new ReactiveProperty<float>(0f);
        IsReleaseShieldAfterExit = true;
        
        _floatModifier = new FloatMultiplierModifier(_guardConfiguration.SlowdownCurve.Evaluate(ShieldValue.Value));
        _horizontalMovement.SpeedModifier.AddModifier(_floatModifier, 0).AddTo(this);
        
        _blockInput.IsHolding.Subscribe(ctx => RaiseShieldValue()).AddTo(this);
        _blockInput.IsHolding.Subscribe(ctx => ReleaseShieldValue()).AddTo(this);
    }

    public override void OnExit()
    {
        if (_coroutine != null) _coroutineHandler.StopCoroutine(_coroutine);
        
        RemoveHitbox();
        _registrar.OnHit -= OnHit;
        _registrar.Dispose();
        
        _animator.ResetTrigger(HitParam);
        if (IsReleaseShieldAfterExit) UpdateShieldValue(0);
        _animator.SetBool(BlockParam, false);
    }
    
    public void UpdateShieldValue(float holdingTime)
    {
        ShieldValue.Value = Mathf.Clamp(holdingTime, 0, _guardConfiguration.MaxHoldingTime) / _guardConfiguration.MaxHoldingTime;
        _animator.SetFloat(ShieldParam, ShieldValue.Value);
        OnShieldValueChanged();
    }
    
    private void RaiseShieldValue()
    {
        if (_blockInput.IsHolding.CurrentValue)
        {
            if (_coroutine != null) _coroutineHandler.StopCoroutine(_coroutine);
            _coroutine = _coroutineHandler.StartCoroutine(Lerp(ShieldValue.Value, _guardConfiguration.MaxHoldingTime, _guardConfiguration.MaxHoldingTime));
        }
    }
    
    private void ReleaseShieldValue()
    {
        if (!_blockInput.IsHolding.CurrentValue)
        {
            if (_coroutine != null) _coroutineHandler.StopCoroutine(_coroutine);
            float duration = ShieldValue.Value * _guardConfiguration.MaxHoldingTime;
            _coroutine = _coroutineHandler.StartCoroutine(Lerp(duration, 0, duration));
        }
    }
    
    private IEnumerator Lerp(float from, float to, float duration)
    {
        var progress = 0f;
        
        while (progress < duration)
        {
            progress += Time.deltaTime;
            var value = Mathf.Lerp(from, to, progress / duration);
            UpdateShieldValue(value);
            
            yield return null;
        }
        UpdateShieldValue(to);
    }

    private void OnShieldValueChanged()
    {
        if (ShieldValue.Value < 0.1) RemoveHitbox();
        else if (ShieldValue.Value > 0.1) AddHitbox();
        
        if (ShieldValue.Value > 0.2f && ShieldValue.Value < 0.7f) _canInterrupt.Value = false;
        else _canInterrupt.Value = true;
        
        _floatModifier.Multiplier = _guardConfiguration.SlowdownCurve.Evaluate(ShieldValue.Value);
        _horizontalMovement.SpeedModifier.UpdateValue();
    }

    private void AddHitbox()
    {
        if (_isShieldActive) return;
        
        _registrar.Activate();
        _isShieldActive = true;
    }

    private void RemoveHitbox()
    {
        if (!_isShieldActive) return;
        
        _isShieldActive = false;
        _registrar.Deactivate();
    }
    
    private void OnHit()
    {
       _animator.SetTrigger(HitParam);
    }
}