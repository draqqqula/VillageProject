using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class ArrowSpawner : ProjectileSpawner
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _interval;
    public override float Spawn(Transform target, Transform origin)
    {
        var arrow = GameObject.Instantiate(_prefab, origin);
        var projectile = arrow.GetComponent<TravellingProjectile>();
        projectile.SetPath(origin, target);
        return _interval;
    }
}