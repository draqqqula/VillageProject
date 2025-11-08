using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

[Serializable]
public class RaycastOriginAttribute : DamageAttributeBase
{
    [SerializeField] private float _maxDistance;

    public class Data
    {
        [Inject(Id = "Raycast")] private Transform _origin;
        public float MaxDistance { get; private set; } = 5;
        public Ray Ray => new Ray(_origin.position, _origin.forward);
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingletonInjectable<Data>();
    }
}