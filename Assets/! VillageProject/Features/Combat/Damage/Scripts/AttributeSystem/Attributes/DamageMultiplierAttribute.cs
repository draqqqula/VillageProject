using System;
using Microsoft.Extensions.DependencyInjection;
using R3;
using UnityEngine;
using Zenject;

[Serializable]
public class DamageMultiplierAttribute : DamageAttributeBase
{
    [SerializeField] private float _multiplyAmount = 1;
    
    public class Data
    {
        public float MultiplyAmount { get; set; }
        private AttackBonus _attackBonus;
        
        [Inject] 
        private void Construct(AttackBonus attackBonus, FirstPersonController player)
        {
            _attackBonus = attackBonus;
            _attackBonus.DamageMultiplierAmount.Subscribe(OnAttackBonusChanged).AddTo(player);
        }
        
        private void OnAttackBonusChanged(float value)
        {
            MultiplyAmount = value;
        }
    }
    
    public override void AddServices(IServiceCollection services)
    {
        services.AddSingletonInjectable(new Data() {MultiplyAmount = _multiplyAmount});
    }
}