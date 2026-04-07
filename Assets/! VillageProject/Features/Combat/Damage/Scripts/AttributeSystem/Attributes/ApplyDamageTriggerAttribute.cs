using System;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

[Serializable]
public class ApplyDamageTriggerAttribute : DamageAttributeBase
{
    public class Effect : IDamageExecutable
    {
        public int GetPriority()
        {
            return 0;
        }

        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.TargetAttributes.TryGetService(out DamageTriggerAttribute.Data sourceAttribute))
            {
                sourceAttribute.Animator.SetTrigger(sourceAttribute.TriggerName);
            }
            return true;
        }
    }
    
    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton<Effect>();
        services.AddSingleton<IDamageExecutable>(provider => provider.GetService<Effect>());
    }
}