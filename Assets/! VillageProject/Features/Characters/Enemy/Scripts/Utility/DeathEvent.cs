using System;
using UnityEngine;
using UnityEngine.Events;

public class DeathEvent : MonoBehaviour
{
    public Action FiredEvent;
    public UnityEvent Fired;
    [SerializeField] private Health _health;

    private void Reset()
    {
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        _health.OnDamageDealt += HandleDamageDealt;
    }

    private void OnDisable()
    {
        _health.OnDamageDealt -= HandleDamageDealt;
    }

    private void HandleDamageDealt(float amount)
    {
        if (_health.Amount <= 0)
        {
            _health.enabled = false;
            FiredEvent?.Invoke();
            Fired?.Invoke();
        }
    }
}
