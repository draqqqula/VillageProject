using System;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IMarkedByIndicator
{
    [field: SerializeField] public Transform OriginPoint { get; private set; }
    
    private DeathEvent _deathEvent;
    public event Action OnDestroyed;

    private void Awake()
    {
        _deathEvent = GetComponent<DeathEvent>();
        _deathEvent.FiredEvent += OnDeath;
    }

    private void OnDeath()
    {
        OnDestroyed?.Invoke();
        _deathEvent.FiredEvent -= OnDeath;
    }
}