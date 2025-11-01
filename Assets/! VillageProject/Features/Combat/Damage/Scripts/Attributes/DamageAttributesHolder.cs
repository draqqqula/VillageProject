using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class DamageAttributesHolder : MonoBehaviour, IDamageAttributeProvider
{
    [Inject] private DiContainer _container;
    private IServiceCollection _services;

    [field: SerializeField] public DamageAttributes Attributes { get; private set; }
    public IServiceProvider Provider { get; private set; }


    public void Awake()
    {
        _services = new ServiceCollection();
        Attributes.AddServices(_services);
        Provider = _services.BuildServiceProvider();

        InjectAttributes();
    }

    private void InjectAttributes()
    {
        var attributes = Provider.GetServices<IDamageInjectable>();

        foreach (var attribute in attributes)
        {
            _container.Inject(attribute);
        }
    }
}