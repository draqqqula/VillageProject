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
    [Inject] private ParryingConfiguration _parryingConfiguration;
    
    [Inject] private CoroutineHandler _coroutineHandler;
    private Coroutine _coroutine;
    
    [Inject] private Animator _animator;
    [Inject] private Stamina _stamina;
    [Inject] private IBlockInput _blockInput;
    [Inject] private ParryingUpdater _parryingUpdater;
    
    [Inject] private HorizontalMovement _horizontalMovement;
    private FloatMultiplierModifier _floatModifier;
    
    [Inject] private HitboxEvent _shieldHitboxEvent;
    private HitRegistrar _registrar;
    
    private ReactiveProperty<bool> _canInterrupt = new ReactiveProperty<bool>(true);
    public ReadOnlyReactiveProperty<bool> CanInterrupt => _canInterrupt;
    public bool IsReleaseShieldAfterExit {private get; set; }

    private bool _isRisedShieldValue = false;
    
    public override void OnEnter()
    {
        _registrar = new HitboxHitRegistrar(_shieldHitboxEvent);
        _registrar.OnHit += OnHit;
        
        _stamina.ModifyRate(_guardConfiguration.StaminaFillModifier).AddTo(this);
        _animator.SetTrigger(BlockParam);
        
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
    }
    
    private void UpdateShieldValue(float shieldValue)
    {
        ShieldValue.Value = Mathf.Clamp(shieldValue, 0, 1);
        _animator.SetFloat(ShieldParam, ShieldValue.Value);
        OnShieldValueChanged();
    }
    
    private void RaiseShieldValue()
    {
        if (_blockInput.IsHolding.CurrentValue)
        {
            _isRisedShieldValue = true;
            if (_coroutine != null) _coroutineHandler.StopCoroutine(_coroutine);
            _coroutine = _coroutineHandler.StartCoroutine(Lerp(ShieldValue.Value, 1, 
                _guardConfiguration.MaxShieldUpTime - (ShieldValue.Value * _guardConfiguration.MaxShieldUpTime)));
        }
    }
    
    private void ReleaseShieldValue()
    {
        if (!_blockInput.IsHolding.CurrentValue)
        {
            _isRisedShieldValue = false;
            if (_coroutine != null) _coroutineHandler.StopCoroutine(_coroutine);
            float duration = ShieldValue.Value * _guardConfiguration.MaxShieldUpTime;
            _coroutine = _coroutineHandler.StartCoroutine(Lerp(ShieldValue.Value, 0, duration));
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
        
        _canInterrupt.Value = true;
        _parryingUpdater.UpdateParryingByShieldParam(ShieldValue.Value, _guardConfiguration.MaxShieldUpTime, _isRisedShieldValue);
        
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
       if (!_parryingUpdater.Parrying) _animator.SetTrigger(HitParam);
    }
}