using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class TowerDamageOverTime : TowerIntervalAction
{
    [Inject] private DiContainer _container;
    public event Action ProjectileFired;
    [SerializeField] private float _defaultInterval;
    public ProjectileSpawner Spawner { get; set; }
    
    [SerializeField] private Building _building;

    protected override float Perform()
    {
        if (_building.Data.CurrentState == BuildingData.State.Wait) return _defaultInterval;
        
        var min = float.MaxValue;
        Health closest = null;
        foreach (var target in Range.Targets.Values)
        {
            var distance = Vector3.Distance(target.transform.position, transform.position);
            if (distance < min)
            {
                min = distance;
                closest = target;
            }
        }

        if (closest != null && Spawner != null)
        {
            var projectile = Spawner.Spawn(_container, closest.transform, transform);
            ProjectileFired?.Invoke();
            return projectile;
        }
        return _defaultInterval;
    }
}