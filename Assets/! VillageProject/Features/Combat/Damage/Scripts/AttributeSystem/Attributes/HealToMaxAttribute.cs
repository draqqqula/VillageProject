using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class HealToMaxAttribute : DamageAttributeBase
{
    public class Effect : IDamageExecutable
    {
        public int GetPriority()
        {
            return 0;
        }

        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.TargetAttributes.TryGetService<IHealth>(out var health))
            {
                health.Amount = health.MaxHealth;
            }
            return true;
        }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton<HealToMaxAttribute>();
    }
}