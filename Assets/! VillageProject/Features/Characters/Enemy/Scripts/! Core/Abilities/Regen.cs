using System;
using System.Collections;
using R3;
using UnityEngine;
using Zenject;

public class Regen : MonoBehaviour
{
    private const float Cooldown = 2f;
    
    [SerializeField] private float _regenSpeed;
    [Inject] private Health _health;

    private float _lastHealthValue;
    
    private Coroutine _regenCoroutine;
    private Coroutine _cooldownCoroutine;
    
    public bool IsOnCooldown {get; private set;}

    private void Awake()
    {
        _health.AmountReactive.Subscribe(CheckHealthValue).AddTo(this);
    }

    private void CheckHealthValue(float amount)
    {
        Debug.Log($"{gameObject.name}'s health: {amount}");
        if (amount < _lastHealthValue) OnDamaged();
        _lastHealthValue = amount;
    }
    
    private void OnDamaged()
    {
        if (_regenCoroutine != null) StopCoroutine(_regenCoroutine);
        if (_cooldownCoroutine != null) StopCoroutine(_cooldownCoroutine);
        
        _cooldownCoroutine = StartCoroutine(RegenCooldownRoutine(ActivateRegen));
    }
    
    private IEnumerator RegenCooldownRoutine(Action callback)
    {
        IsOnCooldown = true;
        yield return new WaitForSeconds(Cooldown);
        IsOnCooldown = false;
        _cooldownCoroutine = null;
        callback?.Invoke();
    }
    
    private void ActivateRegen()
    {
        if (IsOnCooldown) return;
        
        if (_regenCoroutine != null) StopCoroutine(_regenCoroutine);
        _regenCoroutine = StartCoroutine(RegenLerpRoutine());
    }
    
    private IEnumerator RegenLerpRoutine()
    {
        float progress = 0;
        var duration = (_health.MaxHealth - _health.Amount) / _regenSpeed;
        var startHealth = _health.Amount;

        while (progress < duration)
        {
            progress += Time.fixedDeltaTime;
            _health.Amount = Mathf.Lerp(startHealth, _health.MaxHealth, progress / duration);
            yield return null;
        }
        
        _health.Amount = _health.MaxHealth;
        _regenCoroutine = null;
    }
}
