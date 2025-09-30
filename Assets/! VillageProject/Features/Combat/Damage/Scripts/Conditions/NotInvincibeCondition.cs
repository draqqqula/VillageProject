using System;
using UnityEngine;

[Serializable]
public class NotInvincibeCondition : DamageConditionBase
{
    public override bool IsSatisfied(DamageContext context)
    {
        return context.Target.TryGetService(out InvincibilityComponent invincibility)
            && context.Source.TryGetService(out KeyComponent key)
            && !invincibility.IsKeyActive(key.Key);
    }
}
