using System;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

[Serializable]
public class GuardableDamageCondition : DamageAttributeBase
{
    public class Condition : IDamageExecutable
    {
        public bool TryExecute(DamageInteractionContext context)
        {
            return context.SourceAttributes.TryGetService<BlockableDamageAttribute.Data>(out var blockableDamage)
                   && !blockableDamage.IsBlocking;
        }
    }
    
    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton<Condition>();
        services.AddSingleton<IDamageExecutable>(provider => provider.GetService<Condition>());
    }
}