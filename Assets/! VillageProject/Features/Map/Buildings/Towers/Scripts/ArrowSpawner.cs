using System;
using System.Collections;
using UnityEngine;
using Zenject;

[Serializable]
public class ArrowSpawner : ProjectileSpawner
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _interval;
    public override float Spawn(DiContainer container, Transform target, Transform origin)
    {
        var arrow = container.InstantiatePrefab(_prefab, origin);
        var projectile = arrow.GetComponent<TravellingProjectile>();
        projectile.SetPath(origin, target);
        return _interval;
    }
}