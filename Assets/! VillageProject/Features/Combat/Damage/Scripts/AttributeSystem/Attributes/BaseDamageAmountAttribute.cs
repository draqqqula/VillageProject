using Microsoft.Extensions.DependencyInjection;
using System;
using UnityEngine;

[Serializable]
public class BaseDamageAmountAttribute : DamageAttributeBase
{
    [SerializeField] private float _amount;

    public class Effect : IDamageExecutable
    {
        public float Amount { get; set; }
        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.TargetAttributes.TryGetService<HealthAttribute.Service>(out var data))
            {
                data.Health.Amount -= Amount;
            }
            return true;
        }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddScoped(provider => new Effect() { Amount = _amount });
        services.AddScoped<IDamageExecutable>(provider => provider.GetService<Effect>());
    }
}