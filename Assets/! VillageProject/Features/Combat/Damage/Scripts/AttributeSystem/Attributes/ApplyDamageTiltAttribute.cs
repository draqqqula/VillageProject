using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;


[Serializable]
public class ApplyDamageTiltAttribute : DamageAttributeBase
{
    public class Effect : IDamageExecutable
    {
        public int GetPriority()
        {
            return 0;
        }

        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.SourceAttributes.TryGetService(out TransformDamageAttribute.Data sourceTransform)
                && context.TargetAttributes.TryGetService(out TransformDamageAttribute.Data targetTransform)
                && context.TargetAttributes.TryGetService(out DamageTiltableAttribute.Data tiltable))
            {
                tiltable.DamageTilt.Create((sourceTransform.Transform.position - targetTransform.Transform.position).ToXZ().ToVector3XZ());
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