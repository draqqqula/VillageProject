using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using Zenject;

[Serializable]
public class DamageTiltableAttribute : DamageAttributeBase
{
    public class Data
    {
        [Inject] public DamageTilt DamageTilt { get; private set; }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingletonInjectable<Data>();
    }
}