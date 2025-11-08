using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using UnityEngine;
using Zenject;

public class DamageInteractable : MonoBehaviour, IDamageInteractable
{
    [Inject] private DiContainer _container;
    private IServiceCollection _services;

    [field: SerializeField] public DamageAttributes Attributes { get; private set; }
    public IServiceProvider AttributeProvider { get; private set; }

    [field: SerializeField] public DamageRole Role { get; private set; }

    public void Awake()
    {
        _services = new ServiceCollection();
        _services.AddSingleton(_container);
        _services.AddSingleton(gameObject);
        _services.AddSingleton(transform);
        Attributes.AddServices(_services);
        AttributeProvider = _services.BuildServiceProvider();
    }
}