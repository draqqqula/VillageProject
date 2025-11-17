using System.Collections;
using R3;
using UnityEngine;
using Zenject;

public sealed class GuardState : StateBase<GuardState>
{
    private const string HitParam = "Hit";
    private const string ShieldParam = "Shield";
    
    public override StateType StateType => StateType.Guard;
    public GuardStateType GuardType {get; private set;}

    [Inject(Id = "TakeHit")] private IAnimationWindowListener _hitWindow;
    
    public enum GuardStateType
    {
        Guarding, Idle
    }
    
    public ReactiveProperty<float> ShieldValue { get; private set; }
    private bool _isShieldActive;
    private Coroutine _coroutine;
    
    [Inject] private GuardConfiguration _guardConfiguration;
    [Inject] private CoroutineHandler _coroutineHandler;
    [Inject] private Animator _animator;
    [Inject] private Stamina _stamina;
    [Inject] private IBlockInput _blockInput;
    
    [Inject] private HitboxEvent _shieldHitboxEvent;
    private HitRegistrar _registrar;
    
    private bool _isHit;
    
    public override void OnEnter()
    {
        _registrar = new HitboxHitRegistrar(_shieldHitboxEvent);
        _registrar.OnHit += OnHit;
        
        _stamina.ModifyRate(_guardConfiguration.StaminaFillModifier).AddTo(this);
        
        ShieldValue = new ReactiveProperty<float>(0f);
        
        _blockInput.IsHolding.Subscribe(ctx => RaiseShieldValue()).AddTo(this);
        _blockInput.IsHolding.Subscribe(ctx => ReleaseShieldValue()).AddTo(this);
        _hitWindow.OnExit += OnExitHit;
    }

    public override void OnExit()
    {
        if (_coroutine != null) _coroutineHandler.StopCoroutine(_coroutine);
        
        if (_isHit) OnExitHit();
        _hitWindow.OnExit -= OnExitHit;
        _registrar.OnHit -= OnHit;
        
        _registrar.Dispose();
        
        UpdateShieldValue(0);
    }

    public void UpdateShieldValue(float holdingTime)
    {
        ShieldValue.Value = Mathf.Clamp(holdingTime, 0, _guardConfiguration.MaxHoldingTime) / _guardConfiguration.MaxHoldingTime;
        _animator.SetFloat(ShieldParam, ShieldValue.Value);
        ControlShieldAnimation();
        OnShieldValueChanged();
    }

    private void ControlShieldAnimation()
    {
        if (_isHit) return;
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Shield Blend Tree")) return;
        
        _animator.Play("Shield Blend Tree", 0, ShieldValue.Value);
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
    }

    private void AddHitbox()
    {
        if (_isShieldActive) return;

        GuardType = GuardStateType.Guarding;
        _registrar.Activate();
        _isShieldActive = true;
    }

    private void RemoveHitbox()
    {
        if (!_isShieldActive) return;
        
        GuardType = GuardStateType.Idle;

        _isShieldActive = false;
        _registrar.Deactivate();
    }
    
    private void OnHit()
    {
       _isHit = true;
       _animator.SetTrigger(HitParam);
    }

    private void OnExitHit()
    {
        _isHit = false;
    }
}