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
        
        private bool _isInitialized = false;
        
        public bool TryExecute(DamageInteractionContext context)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                _weakSpotController.IsOpened.Subscribe(OnSpotChanged).AddTo(_weakSpotController.gameObject);
            }
            return true;
        }

        private void OnSpotChanged(bool isOpened)
        {
            if (isOpened) _blowUpAbilitiy.ActivateBlowUpWithCharging();
            else _blowUpAbilitiy.Interrupt();
        }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingletonInjectable<Effect>();
        services.AddSingleton<IDamageExecutable>(provider => provider.GetService<Effect>());
    }
}