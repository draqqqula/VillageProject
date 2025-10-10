using System;
using UnityEngine;

public class Origin 
{
    public Transform OriginPoint { get; private set; }
    
    private IndicatorController _indicatorController;
    
    private DeathEvent _deathEvent;
    public event Action OnDestroyed;

    public Origin(Transform originPoint, DeathEvent deathEvent, IndicatorController indicatorController)
    {
        OriginPoint = originPoint;
        
        _indicatorController = indicatorController;
        _indicatorController.AddOrigin(this);
        
        _deathEvent = deathEvent;
        _deathEvent.FiredEvent += OnDeath;
    }
    
    private void OnDeath()
    {
        _indicatorController.RemoveOrigin(this);
        OnDestroyed?.Invoke();
        _deathEvent.FiredEvent -= OnDeath;
    }
}