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

        public int GetPriority()
        {
            return 20;
        }
        
        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.TargetAttributes.TryGetService<IHealthAttribute>(out var data))
            {
                if (context.SourceAttributes.TryGetService<DamageMultiplierAttribute.Data>(out var damageMultiplier))
                {
                    data.Health.Amount -= Amount * damageMultiplier.MultiplyAmount;
                }
                else data.Health.Amount -= Amount;
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