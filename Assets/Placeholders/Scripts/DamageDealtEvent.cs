using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using R3;

[RequireComponent(typeof(Health))]
public class DamageDealtEvent : MonoBehaviour
{
    public UnityEvent<float> Callback;
    [SerializeField] private Health _health;
    private void Reset()
    {
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        _health.OnDamageDealt += HandleDamage;
    }

    private void HandleDamage(float health)
    {
        Callback?.Invoke(health);
    }
}