using System;
using Microsoft.Extensions.DependencyInjection;
using R3;
using UnityEngine;
using Zenject;

[Serializable]
public class BlowUpEffectAttribute : DamageAttributeBase
{
    public class Effect : IDamageExecutable
    {
        [Inject] private BlowUp _blowUpAbilitiy;
        [Inject] private WeakSpotController _weakSpotController;
        
        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.SourceAttributes.TryGetService<BaseDamageAmountAttribute.Effect>(out var baseDamageAmount))
            {
                Debug.Log("Activate BlowUp Effect");
                _blowUpAbilitiy.ActivateBlowUp();
                _weakSpotController.Open();
                _weakSpotController.IsOpened.Subscribe(TryInterrupt).AddTo(_weakSpotController.gameObject);
            }
            return true;
        }

        private void TryInterrupt(bool value)
        {
            if (!value) _blowUpAbilitiy.Interrupt();
        }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingletonInjectable<Effect>();
        services.AddSingleton<IDamageExecutable>(provider => provider.GetService<Effect>());
    }
}