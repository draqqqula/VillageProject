using System;
using UnityEngine;

[Serializable]
public class GuardableDamageCondition : DamageConditionBase
{
    public override bool IsSatisfied(DamageContext context)
    {
        return context.Source.TryGetService<BlockableDamageComponent>(out var blockableDamage)
               && !blockableDamage.IsBlocking;
    }
}