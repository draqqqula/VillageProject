using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class HitWeakSpotEffect : DamageEffectBase
{
    public class CriticalHit
    {
        public bool Flag = false;
    }

    [SerializeField] private float _modifier = 1f;
    public override float Apply(DamageContext context, float baseDamage)
    {
        if (context.Source.TryGetService(out RaycastOriginDamageComponent raycastOrigin)
            && context.Target.TryGetService(out WeakSpotDamageComponent weakSpot)
            && context.Source.TryGetService(out CriticalHit criticalHit)
            && weakSpot.TryHit(raycastOrigin.Ray, raycastOrigin.MaxDistance))
        {
            criticalHit.Flag = true;
            return baseDamage * _modifier;
        }
        return baseDamage;
    }

    public override void RegisterTo(IServiceCollection services)
    {
        base.RegisterTo(services);
        services.AddScoped<CriticalHit>();
    }
}