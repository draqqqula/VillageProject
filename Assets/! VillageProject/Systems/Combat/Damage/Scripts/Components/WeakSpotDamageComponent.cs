using System;
using UnityEngine;

[Serializable]
public class WeakSpotDamageComponent : DamageComponentBase
{
    [SerializeField] private WeakSpot _body;

    public bool TryHit(Ray ray, float maxDistance)
    {
        if (_body.Raycast(ray, maxDistance))
        {
            _body.Close();
            return true;
        }
        return false;
    }
}
