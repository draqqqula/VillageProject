using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

[Serializable]
public class GainAdrenalineAttribute : DamageAttributeBase
{
    public class Effect : IDamageExecutable
    {
        [Inject] private Adrenaline _adrenaline;

        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.TargetAttributes.TryGetService(out AdrenalineAmountAttribute.Data amount))
            {
                if (context.SourceAttributes.TryGetService(out CriticalHitAttribute.Effect criticalHit)
                    && criticalHit.Successful)
                {
                    _adrenaline.Gain(amount.AmountForCritical, amount.Cooldown);
                }
                else
                {
                    _adrenaline.Gain(amount.AmountForBasic, amount.Cooldown);
                }
            }
            return true;
        }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingletonInjectable<Effect>();
        services.AddSingleton<IDamageExecutable>(provider => provider.GetService<Effect>());
    }
}