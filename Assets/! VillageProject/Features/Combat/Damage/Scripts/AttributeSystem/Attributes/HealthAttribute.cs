using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

[Serializable]
public class HealthAttribute : DamageAttributeBase
{
    public class Service : IHealthAttribute
    {
        [Inject] public IHealth Health { get; private set; }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingletonInjectable<IHealthAttribute, Service>();
    }
}