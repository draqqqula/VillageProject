using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;


[Serializable]
public class CriticalHitAttribute : DamageAttributeBase
{
    [SerializeField] private float _modifier;

    public class Effect : IDamageExecutable
    {
        public Effect(float modifier)
        {
            Modifier = modifier;
        }

        public bool Successful { get; private set; } = false;
        public float Modifier { get; private set; }
        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.SourceAttributes.TryGetService(out RaycastOriginAttribute.Data raycastOrigin)
                && context.TargetAttributes.TryGetService(out WeakSpotDamageAttribute.Data weakSpot)
                && context.SourceAttributes.TryGetService(out BaseDamageAmountAttribute.Effect damage)
                && weakSpot.TryHit(raycastOrigin.Ray, raycastOrigin.MaxDistance))
            {
                Successful = true;
                damage.Amount *= Modifier;
            }
            return true;
        }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddScoped<Effect>(provider => new Effect(_modifier));
        services.AddScoped<IDamageExecutable>(provider => provider.GetService<Effect>());
    }
}