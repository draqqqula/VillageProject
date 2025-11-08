using System;
using System.Collections;
using UnityEngine;
using Zenject;

[Serializable]
public class MultiArrowSpawner : ProjectileSpawner
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _interval;
    [SerializeField] private int _amount;
    [SerializeField] private float _spread;
    [SerializeField] private AnimationCurve _spreadCurve;

    public override float Spawn(DiContainer container, Transform target, Transform origin)
    {
        for (int i = 0; i < _amount; i++)
        {
            var arrow = container.InstantiatePrefab(_prefab, origin);
            var projectile = arrow.GetComponent<StraightLineProjectile>();
            projectile.transform.LookAt(target);
            var angle = UnityEngine.Random.Range(0.0f, 360.0f);
            var delta = new Vector2(
                Mathf.Cos(angle) - Mathf.Sin(angle),
                Mathf.Sin(angle) + Mathf.Cos(angle)
            ) * RandomSpread();
            projectile.transform.Rotate(Vector3.right, delta.x);
            projectile.transform.Rotate(Vector3.up, delta.y);
        }
        return _interval;
    }

    private float RandomSpread()
    {
        return _spreadCurve.Evaluate(UnityEngine.Random.Range(0, 1f)) * _spread;
    }
}