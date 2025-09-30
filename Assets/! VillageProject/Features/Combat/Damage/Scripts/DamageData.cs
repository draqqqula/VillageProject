using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DamageData
{
    [SerializeReference, SubclassSelector] public List<DamageComponentBase> Components;
    [SerializeReference, SubclassSelector] public List<DamageConditionBase> Conditions;
    [SerializeReference, SubclassSelector] public List<DamageEffectBase> Effects;

    public void RegisterTo(IServiceCollection services)
    {
        foreach (var component in Components)
        {
            component.RegisterTo(services);
        }
        foreach (var component in Conditions)
        {
            component.RegisterTo(services);
        }
        foreach (var component in Effects)
        {
            component.RegisterTo(services);
        }
    }
}