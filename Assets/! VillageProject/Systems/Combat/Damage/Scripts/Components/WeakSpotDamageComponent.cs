using System;
using UnityEngine;

[Serializable]
public class WeakSpotDamageComponent : DamageComponentBase
{
    [SerializeField] private WeakSpot _body;

    public bool Raycast(Ray ray, float maxDistance)
    {
        return _body.Raycast(ray, maxDistance);
    }
}
