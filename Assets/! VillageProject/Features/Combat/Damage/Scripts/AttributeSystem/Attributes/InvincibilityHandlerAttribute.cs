using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InvincibilityHandlerAttribute : DamageAttributeBase
{
    public class Condition : IDamageExecutable
    {
        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.SourceAttributes.TryGetService<Storage>(out var storage)
                && context.SourceAttributes.TryGetService<KeyAttribute>(out var key)
                && storage.IsKeyActive(key.Key))
            {
                return false;
            }
            return true;
        }
    }

    public class Storage
    {
        public Storage(float defaultDuration)
        {
            DefaultDuration = defaultDuration;
        }

        private Dictionary<string, float> _expireStamps = new Dictionary<string, float>();

        public float DefaultDuration { get; private set; }

        public void SetKey(string key, float durationFactor)
        {
            _expireStamps[key] = Time.time + DefaultDuration * durationFactor;
        }

        public bool IsKeyActive(string key)
        {
            if (_expireStamps.TryGetValue(key, out var expiration))
            {
                return Time.time <= expiration;
            }
            return false;
        }
    }

    [field: SerializeField] public float DefaultDuration { get; private set; }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton(new Storage(DefaultDuration));
        services.AddSingleton<Condition>();
        services.AddSingleton<IDamageExecutable>(provider => provider.GetService<Condition>());
    }
}