using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

[Serializable]
public class TransformDamageAttribute : DamageAttributeBase
{
    public class Data
    {
        public Data(Transform transform)
        {
            Transform = transform;
        }

        public Transform Transform { get; private set; }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton<Data>();
    }
}