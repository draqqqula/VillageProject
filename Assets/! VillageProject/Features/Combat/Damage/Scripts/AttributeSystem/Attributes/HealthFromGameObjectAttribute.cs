using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class HealthFromGameObjectAttribute : DamageAttributeBase
{
    public class Data : IHealthAttribute
    {
        public Data(GameObject gameObject)
        {
            Health = gameObject.GetComponent<Health>();
        }
        public IHealth Health { get; set; }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IHealthAttribute, Data>();
    }
}