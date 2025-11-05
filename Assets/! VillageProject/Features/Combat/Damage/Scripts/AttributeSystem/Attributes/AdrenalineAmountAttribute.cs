using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class AdrenalineAmountAttribute : DamageAttributeBase
{
    public class Data
    {
        public float AmountForBasic { get; private set; }
        public float AmountForCritical { get; private set; }
        public float Cooldown { get; private set; }

        public Data(float amountForBasic, float amountForCritical, float cooldown)
        {
            AmountForBasic = amountForBasic;
            AmountForCritical = amountForCritical;
            Cooldown = cooldown;
        }
    }

    [SerializeField] private float _amountForBasic;
    [SerializeField] private float _amountForCritical;
    [SerializeField] private float _cooldown;

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton<Data>(new Data(_amountForBasic, _amountForCritical, _cooldown));
    }
}