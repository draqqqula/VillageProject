using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class HitWeakSpotEffect : DamageEffectBase
{
    [SerializeField] private float _modifier = 1f;
    public override float Apply(DamageContext context, float baseDamage)
    {
        if (context.Source.TryGetComponent(out RaycastOriginDamageComponent raycastOrigin)
            && context.Target.TryGetComponent(out WeakSpotDamageComponent weakSpot)
            && weakSpot.Raycast(raycastOrigin.Ray, raycastOrigin.MaxDistance))
        {
            return baseDamage * _modifier;
        }
        return baseDamage;
    }
}