using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    private IDamageComponentProviderFactory _factory = new DefaultDamageComponentProviderFactory();
    [field: SerializeField] public DamageData Data { get; private set; }
    [field: SerializeField] public DamageInfo Info { get; private set; }
    public IServiceProvider ComponentProvider { get; private set; }

    private void Awake()
    {
        var services = new ServiceCollection();
        Data.RegisterTo(services);
        Info.Data.RegisterTo(services);
        ComponentProvider = services.BuildServiceProvider();
    }
}
