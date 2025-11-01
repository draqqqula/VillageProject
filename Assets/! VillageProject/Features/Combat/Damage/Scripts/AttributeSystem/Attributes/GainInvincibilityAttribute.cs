using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class GainInvincibilityAttribute : DamageAttributeBase
{
    [SerializeField] private float _durationFactor;

    public class Effect : IDamageExecutable
    {
        public Effect(float durationFactor)
        {
            DurationFactor = durationFactor;
        }

        public float DurationFactor { get; private set; }
        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.SourceAttributes.TryGetService<InvincibilityHandlerAttribute.Storage>(out var storage)
                && context.SourceAttributes.TryGetService<KeyAttribute>(out var key))
            {
                storage.SetKey(key.Key, DurationFactor);
            }
            return true;
        }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton(provider => new Effect(_durationFactor));
        services.AddSingleton<IDamageExecutable>(provider => provider.GetService<Effect>());
    }
}