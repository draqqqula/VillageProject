using System.Collections;
using R3;
using UnityEngine;
using Zenject;

public sealed class GuardState : StateBase<GuardState>
{
    private const string HitParam = "Hit";
    private const string ShieldParam = "Shield";
    
    public override StateType StateType => StateType.Guard;
    
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
    
    public override void OnEnter()
    {
        _registrar = new HitboxHitRegistrar(_shieldHitboxEvent);
        
        _stamina.ModifyRate(_guardConfiguration.StaminaFillModifier).AddTo(this);
        
        ShieldValue = new ReactiveProperty<float>(0f);
        _blockInput.CurrentHoldTime.Subscribe(UpdateShieldValue).AddTo(this);
        _blockInput.IsHolding.Subscribe(ctx => ReleaseShieldValue()).AddTo(this);
    }

    public override void OnExit() { }

    public void UpdateShieldValue(float holdingTime)
    {
        ShieldValue.Value = Mathf.Clamp(holdingTime, 0, _guardConfiguration.MaxHoldingTime) / _guardConfiguration.MaxHoldingTime;
        _animator.SetFloat(ShieldParam, ShieldValue.Value);
        OnShieldValueChanged();
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
            progress += Time.fixedDeltaTime / duration;
            var value = Mathf.Lerp(from, to, progress);
            UpdateShieldValue(value);
            
            if (ShieldValue.Value == 0) break;
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
        
        _registrar.Activate();
        _registrar.OnHit += OnHit;
        _isShieldActive = true;
    }

    private void RemoveHitbox()
    {
        if (!_isShieldActive) return;
        
        _registrar.OnHit -= OnHit;

        _isShieldActive = false;
        _registrar.Deactivate();
        _registrar.Dispose();
    }
    
    private void OnHit()
    {
        _animator.SetTrigger(HitParam);
    }
}