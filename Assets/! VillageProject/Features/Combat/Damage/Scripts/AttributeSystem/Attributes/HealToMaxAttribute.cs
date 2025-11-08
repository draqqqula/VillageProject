using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class HealToMaxAttribute : DamageAttributeBase
{
    public class Effect : IDamageExecutable
    {
        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.TargetAttributes.TryGetService<IHealth>(out var health))
            {
                health.Amount = health.MaxAmount;
            }
            return true;
        }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton<HealToMaxAttribute>();
    }
}