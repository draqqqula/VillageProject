using System;
using Microsoft.Extensions.DependencyInjection;
using Zenject;

[Serializable]
public class GuardEffectAttribute : DamageAttributeBase
{
    public class Effect : IDamageExecutable
    {
        [Inject] private Stamina _stamina;
        [Inject] private GuardConfiguration _guardConfiguration;
        [Inject] private ParryingUpdater _parryingUpdater;
        
        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.SourceAttributes.TryGetService<BlockableDamageAttribute.Data>(out var blockableDamage)
                && context.SourceAttributes.TryGetService<BaseDamageAmountAttribute.Effect>(out var baseDamageAmount))
            {
                if (_parryingUpdater.Parrying.CurrentValue || _stamina.TrySpend(_guardConfiguration.StaminaWasteModifier * baseDamageAmount.Amount))
                    blockableDamage.Apply();
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