using UnityEngine;
using System;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine.Serialization;
using Zenject;

[Serializable]
public class BlockableDamageAttribute : DamageAttributeBase
{
    public class Data
    {
        [Inject] public StateOfAttack StateOfAttack {get; private set;}
        public bool IsBlocking => StateOfAttack.IsBlocking;
        
        public void Apply()
        {
            StateOfAttack.IsBlocking = true;
        }
    }
    
    public override void AddServices(IServiceCollection services)
    {
        services.AddSingletonInjectable<Data>();
    }
}