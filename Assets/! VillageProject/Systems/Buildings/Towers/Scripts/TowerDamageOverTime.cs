using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerDamageOverTime : TowerIntervalAction
{
    public event Action ProjectileFired;
    [SerializeField] private float _defaultInterval;
    public ProjectileSpawner Spawner { get; set; }

    protected override float Perform()
    {
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
            var projectile = Spawner.Spawn(closest.transform, transform);
            ProjectileFired?.Invoke();
            return projectile;
        }
        return _defaultInterval;
    }
}
