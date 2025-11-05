using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

[Serializable]
public class WeakSpotDamageAttribute : DamageAttributeBase
{
    public class Data
    {
        [Inject] private WeakSpotController _body;

        public bool TryHit(Ray ray, float maxDistance)
        {
            if (_body.Raycast(ray, maxDistance))
            {
                _body.Close();
                return true;
            }
            return false;
        }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingletonInjectable<Data>();
    }
}