using System;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;
using Zenject;

[Serializable]
public class WeakSpotWhenParryingAttribute : DamageAttributeBase
{
    public class Data
    {
        [Inject] WeakSpotController _weakSpotController;
        
        public void Apply()
        {
            if (!_weakSpotController.IsOpened.CurrentValue) _weakSpotController.Open();    
        }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingletonInjectable<Data>();
    }
}