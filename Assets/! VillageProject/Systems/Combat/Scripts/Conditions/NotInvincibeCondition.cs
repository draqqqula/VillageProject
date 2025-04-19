using System;
using UnityEngine;

[Serializable]
public class NotInvincibeCondition : DamageConditionBase
{
    public override bool IsSatisfied(DamageContext context)
    {
        return context.Target.TryGetComponent(out InvincibilityComponent invincibility)
            && context.Source.TryGetComponent(out KeyComponent key)
            && !invincibility.IsKeyActive(key.Key);
    }
}
