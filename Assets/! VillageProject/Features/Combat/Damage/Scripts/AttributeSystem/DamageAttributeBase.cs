using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;


[Serializable]
public abstract class DamageAttributeBase
{
    public abstract void AddServices(IServiceCollection services);
}