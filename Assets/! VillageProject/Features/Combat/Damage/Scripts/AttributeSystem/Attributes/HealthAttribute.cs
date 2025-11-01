using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

[Serializable]
public class HealthAttribute : DamageAttributeBase
{
    public class Service
    {
        [Inject] public IHealth Health { get; private set; }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingletonInjectable<Service, Service>();
    }
}