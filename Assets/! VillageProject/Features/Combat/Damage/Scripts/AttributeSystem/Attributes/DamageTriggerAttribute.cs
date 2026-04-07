using System;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;
using Zenject;

[Serializable]
public class DamageTriggerAttribute : DamageAttributeBase
{
    [SerializeField] private string _triggerName;
    
    public class Data
    {
        [Inject(Optional = true)] private Villager _villager;
        [Inject(Optional = true)] private Animator _animator;
        
        public string TriggerName { get; set; }
        public Animator Animator => _animator ?? _villager.SkinReferencesResolver.CurrentValue.Animator;
    }
    
    public override void AddServices(IServiceCollection services)
    {
        services.AddSingletonInjectable(new Data() {TriggerName = _triggerName});
    }
}