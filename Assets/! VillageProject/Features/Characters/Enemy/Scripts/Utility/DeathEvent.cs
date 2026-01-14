using System;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class DeathEvent : MonoBehaviour
{
    public Action FiredEvent;
    public UnityEvent Fired;
    [SerializeField] protected Health _health;

    private void Reset()
    {
        _health = GetComponent<Health>();
    }

    protected virtual void OnEnable()
    {
        _health.OnDamageDealt += HandleDamageDealt;
    }

    protected virtual void OnDisable()
    {
        _health.OnDamageDealt -= HandleDamageDealt;
    }

    protected virtual void HandleDamageDealt(float amount)
    {
        if (_health.Amount <= 0)
        {
            _health.enabled = false;
            FiredEvent?.Invoke();
            Fired?.Invoke();
        }
    }
}