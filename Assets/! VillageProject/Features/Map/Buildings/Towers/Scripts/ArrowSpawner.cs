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
        return Spawn(container, target, origin, out GameObject spawnedObject);
    }

    public float Spawn(DiContainer container, Transform target, Transform origin, out GameObject spawnedObject)
    {
        var arrow = container.InstantiatePrefab(_prefab, origin);
        spawnedObject = arrow;
        
        var projectile = arrow.GetComponent<TravellingProjectile>();
        projectile.SetPath(origin, target);
        return _interval;
    }
}