using System.Collections;
using UnityEngine;
using System;
using Microsoft.Extensions.DependencyInjection;

[AttributeUsage(AttributeTargets.Class)]
public class GenerateDamageAttributeAttribute : Attribute
{
    public GenerateDamageAttributeAttribute(ServiceLifetime lifetime = ServiceLifetime.Singleton, bool inject = false)
    {
        Lifetime = lifetime;
        Inject = inject;
    }

    public ServiceLifetime Lifetime { get; private set; }
    public bool Inject { get; private set; }
}