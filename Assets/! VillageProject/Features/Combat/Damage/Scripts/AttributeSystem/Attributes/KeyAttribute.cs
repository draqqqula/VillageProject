using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class KeyAttribute : DamageAttributeBase
{
    [field: SerializeField] public string Key { get; private set; }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton(this);
    }
}